using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Internal;
using Net40.Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection.ServiceLookup;

internal class CallSiteFactory
{
	private struct ServiceDescriptorCacheItem
	{
		private ServiceDescriptor _item;

		private List<ServiceDescriptor> _items;

		public ServiceDescriptor Last
		{
			get
			{
				if (_items != null && _items.Count > 0)
				{
					return _items[_items.Count - 1];
				}
				Debug.Assert(_item != null);
				return _item;
			}
		}

		public int Count
		{
			get
			{
				if (_item == null)
				{
					Debug.Assert(_items == null);
					return 0;
				}
				return 1 + (_items?.Count ?? 0);
			}
		}

		public ServiceDescriptor this[int index]
		{
			get
			{
				if (index >= Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				if (index == 0)
				{
					return _item;
				}
				return _items[index - 1];
			}
		}

		public int GetSlot(ServiceDescriptor descriptor)
		{
			if (descriptor == _item)
			{
				return 0;
			}
			if (_items != null)
			{
				int index = _items.IndexOf(descriptor);
				if (index != -1)
				{
					return index + 1;
				}
			}
			throw new InvalidOperationException("Requested service descriptor doesn't exist.");
		}

		public ServiceDescriptorCacheItem Add(ServiceDescriptor descriptor)
		{
			ServiceDescriptorCacheItem newCacheItem = default(ServiceDescriptorCacheItem);
			if (_item == null)
			{
				Debug.Assert(_items == null);
				newCacheItem._item = descriptor;
			}
			else
			{
				newCacheItem._item = _item;
				newCacheItem._items = _items ?? new List<ServiceDescriptor>();
				newCacheItem._items.Add(descriptor);
			}
			return newCacheItem;
		}
	}

	private const int DefaultSlot = 0;

	private readonly List<ServiceDescriptor> _descriptors;

	private readonly ConcurrentDictionary<Type, ServiceCallSite> _callSiteCache = new ConcurrentDictionary<Type, ServiceCallSite>();

	private readonly Dictionary<Type, ServiceDescriptorCacheItem> _descriptorLookup = new Dictionary<Type, ServiceDescriptorCacheItem>();

	private readonly StackGuard _stackGuard;

	public CallSiteFactory(IEnumerable<ServiceDescriptor> descriptors)
	{
		_stackGuard = new StackGuard();
		_descriptors = descriptors.ToList();
		Populate();
	}

	private void Populate()
	{
		foreach (ServiceDescriptor descriptor in _descriptors)
		{
			TypeInfo serviceTypeInfo = IntrospectionExtensions.GetTypeInfo(descriptor.ServiceType);
			if (serviceTypeInfo.IsGenericTypeDefinition)
			{
				Type implementationType = descriptor.ImplementationType;
				TypeInfo implementationTypeInfo = (((object)implementationType != null) ? IntrospectionExtensions.GetTypeInfo(implementationType) : null);
				if (implementationTypeInfo == null || !implementationTypeInfo.IsGenericTypeDefinition)
				{
					throw new ArgumentException(Resources.FormatOpenGenericServiceRequiresOpenGenericImplementation(descriptor.ServiceType), "descriptors");
				}
				if (implementationTypeInfo.IsAbstract || implementationTypeInfo.IsInterface)
				{
					throw new ArgumentException(Resources.FormatTypeCannotBeActivated(descriptor.ImplementationType, descriptor.ServiceType));
				}
			}
			else if (descriptor.ImplementationInstance == null && descriptor.ImplementationFactory == null)
			{
				Debug.Assert(descriptor.ImplementationType != null);
				TypeInfo implementationTypeInfo2 = IntrospectionExtensions.GetTypeInfo(descriptor.ImplementationType);
				if (implementationTypeInfo2.IsGenericTypeDefinition || implementationTypeInfo2.IsAbstract || implementationTypeInfo2.IsInterface)
				{
					throw new ArgumentException(Resources.FormatTypeCannotBeActivated(descriptor.ImplementationType, descriptor.ServiceType));
				}
			}
			Type cacheKey = descriptor.ServiceType;
			_descriptorLookup.TryGetValue(cacheKey, out var cacheItem);
			_descriptorLookup[cacheKey] = cacheItem.Add(descriptor);
		}
	}

	internal ServiceCallSite GetCallSite(Type serviceType, CallSiteChain callSiteChain)
	{
		return _callSiteCache.GetOrAdd(serviceType, (Type type) => CreateCallSite(type, callSiteChain));
	}

	internal ServiceCallSite GetCallSite(ServiceDescriptor serviceDescriptor, CallSiteChain callSiteChain)
	{
		if (_descriptorLookup.TryGetValue(serviceDescriptor.ServiceType, out var descriptor))
		{
			return TryCreateExact(serviceDescriptor, serviceDescriptor.ServiceType, callSiteChain, descriptor.GetSlot(serviceDescriptor));
		}
		Debug.Fail("_descriptorLookup didn't contain requested serviceDescriptor");
		return null;
	}

	private ServiceCallSite CreateCallSite(Type serviceType, CallSiteChain callSiteChain)
	{
		if (!_stackGuard.TryEnterOnCurrentStack())
		{
			return _stackGuard.RunOnEmptyStack((Type type, CallSiteChain chain) => CreateCallSite(type, chain), serviceType, callSiteChain);
		}
		callSiteChain.CheckCircularDependency(serviceType);
		ServiceCallSite callSite = TryCreateExact(serviceType, callSiteChain) ?? TryCreateOpenGeneric(serviceType, callSiteChain) ?? TryCreateEnumerable(serviceType, callSiteChain);
		_callSiteCache[serviceType] = callSite;
		return callSite;
	}

	private ServiceCallSite TryCreateExact(Type serviceType, CallSiteChain callSiteChain)
	{
		if (_descriptorLookup.TryGetValue(serviceType, out var descriptor))
		{
			return TryCreateExact(descriptor.Last, serviceType, callSiteChain, 0);
		}
		return null;
	}

	private ServiceCallSite TryCreateOpenGeneric(Type serviceType, CallSiteChain callSiteChain)
	{
		if (serviceType.IsConstructedGenericType() && _descriptorLookup.TryGetValue(serviceType.GetGenericTypeDefinition(), out var descriptor))
		{
			return TryCreateOpenGeneric(descriptor.Last, serviceType, callSiteChain, 0);
		}
		return null;
	}

	private ServiceCallSite TryCreateEnumerable(Type serviceType, CallSiteChain callSiteChain)
	{
		try
		{
			callSiteChain.Add(serviceType);
			if (serviceType.IsConstructedGenericType() && serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
			{
				Type itemType = serviceType.GenericTypeArguments().Single();
				CallSiteResultCacheLocation cacheLocation = CallSiteResultCacheLocation.Root;
				List<ServiceCallSite> callSites = new List<ServiceCallSite>();
				if (!itemType.IsConstructedGenericType() && _descriptorLookup.TryGetValue(itemType, out var descriptors))
				{
					for (int i = 0; i < descriptors.Count; i++)
					{
						ServiceDescriptor descriptor = descriptors[i];
						int slot = descriptors.Count - i - 1;
						ServiceCallSite callSite = TryCreateExact(descriptor, itemType, callSiteChain, slot);
						Debug.Assert(callSite != null);
						cacheLocation = GetCommonCacheLocation(cacheLocation, callSite.Cache.Location);
						callSites.Add(callSite);
					}
				}
				else
				{
					int slot2 = 0;
					for (int j = _descriptors.Count - 1; j >= 0; j--)
					{
						ServiceDescriptor descriptor2 = _descriptors[j];
						ServiceCallSite callSite2 = TryCreateExact(descriptor2, itemType, callSiteChain, slot2) ?? TryCreateOpenGeneric(descriptor2, itemType, callSiteChain, slot2);
						if (callSite2 != null)
						{
							slot2++;
							cacheLocation = GetCommonCacheLocation(cacheLocation, callSite2.Cache.Location);
							callSites.Add(callSite2);
						}
					}
					callSites.Reverse();
				}
				ResultCache resultCache = ResultCache.None;
				if (cacheLocation == CallSiteResultCacheLocation.Scope || cacheLocation == CallSiteResultCacheLocation.Root)
				{
					resultCache = new ResultCache(cacheLocation, new ServiceCacheKey(serviceType, 0));
				}
				return new IEnumerableCallSite(resultCache, itemType, callSites.ToArray());
			}
			return null;
		}
		finally
		{
			callSiteChain.Remove(serviceType);
		}
	}

	private CallSiteResultCacheLocation GetCommonCacheLocation(CallSiteResultCacheLocation locationA, CallSiteResultCacheLocation locationB)
	{
		return (CallSiteResultCacheLocation)Math.Max((int)locationA, (int)locationB);
	}

	private ServiceCallSite TryCreateExact(ServiceDescriptor descriptor, Type serviceType, CallSiteChain callSiteChain, int slot)
	{
		if (serviceType == descriptor.ServiceType)
		{
			ResultCache lifetime = new ResultCache(descriptor.Lifetime, serviceType, slot);
			ServiceCallSite callSite;
			if (descriptor.ImplementationInstance != null)
			{
				callSite = new ConstantCallSite(descriptor.ServiceType, descriptor.ImplementationInstance);
			}
			else if (descriptor.ImplementationFactory != null)
			{
				callSite = new FactoryCallSite(lifetime, descriptor.ServiceType, descriptor.ImplementationFactory);
			}
			else
			{
				if (!(descriptor.ImplementationType != null))
				{
					throw new InvalidOperationException("Invalid service descriptor");
				}
				callSite = CreateConstructorCallSite(lifetime, descriptor.ServiceType, descriptor.ImplementationType, callSiteChain);
			}
			return callSite;
		}
		return null;
	}

	private ServiceCallSite TryCreateOpenGeneric(ServiceDescriptor descriptor, Type serviceType, CallSiteChain callSiteChain, int slot)
	{
		if (serviceType.IsConstructedGenericType() && serviceType.GetGenericTypeDefinition() == descriptor.ServiceType)
		{
			Debug.Assert(descriptor.ImplementationType != null, "descriptor.ImplementationType != null");
			ResultCache lifetime = new ResultCache(descriptor.Lifetime, serviceType, slot);
			Type closedType = descriptor.ImplementationType.MakeGenericType(serviceType.GenericTypeArguments());
			return CreateConstructorCallSite(lifetime, serviceType, closedType, callSiteChain);
		}
		return null;
	}

	private ServiceCallSite CreateConstructorCallSite(ResultCache lifetime, Type serviceType, Type implementationType, CallSiteChain callSiteChain)
	{
		try
		{
			callSiteChain.Add(serviceType, implementationType);
			ConstructorInfo[] constructors = IntrospectionExtensions.GetTypeInfo(implementationType).DeclaredConstructors.Where((ConstructorInfo constructor) => constructor.IsPublic).ToArray();
			ServiceCallSite[] parameterCallSites = null;
			if (constructors.Length == 0)
			{
				throw new InvalidOperationException(Resources.FormatNoConstructorMatch(implementationType));
			}
			if (constructors.Length == 1)
			{
				ConstructorInfo constructor2 = constructors[0];
				ParameterInfo[] parameters = constructor2.GetParameters();
				if (parameters.Length == 0)
				{
					return new ConstructorCallSite(lifetime, serviceType, constructor2);
				}
				parameterCallSites = CreateArgumentCallSites(serviceType, implementationType, callSiteChain, parameters, throwIfCallSiteNotFound: true);
				return new ConstructorCallSite(lifetime, serviceType, constructor2, parameterCallSites);
			}
			Array.Sort(constructors, (ConstructorInfo a, ConstructorInfo b) => b.GetParameters().Length.CompareTo(a.GetParameters().Length));
			ConstructorInfo bestConstructor = null;
			HashSet<Type> bestConstructorParameterTypes = null;
			for (int i = 0; i < constructors.Length; i++)
			{
				ParameterInfo[] parameters2 = constructors[i].GetParameters();
				ServiceCallSite[] currentParameterCallSites = CreateArgumentCallSites(serviceType, implementationType, callSiteChain, parameters2, throwIfCallSiteNotFound: false);
				if (currentParameterCallSites == null)
				{
					continue;
				}
				if (bestConstructor == null)
				{
					bestConstructor = constructors[i];
					parameterCallSites = currentParameterCallSites;
					continue;
				}
				if (bestConstructorParameterTypes == null)
				{
					bestConstructorParameterTypes = new HashSet<Type>(from p in bestConstructor.GetParameters()
						select p.ParameterType);
				}
				if (!bestConstructorParameterTypes.IsSupersetOf(parameters2.Select((ParameterInfo p) => p.ParameterType)))
				{
					string message = string.Join(Environment.NewLine, Resources.FormatAmbiguousConstructorException(implementationType), bestConstructor, constructors[i]);
					throw new InvalidOperationException(message);
				}
			}
			if (bestConstructor == null)
			{
				throw new InvalidOperationException(Resources.FormatUnableToActivateTypeException(implementationType));
			}
			Debug.Assert(parameterCallSites != null);
			return new ConstructorCallSite(lifetime, serviceType, bestConstructor, parameterCallSites);
		}
		finally
		{
			callSiteChain.Remove(serviceType);
		}
	}

	private ServiceCallSite[] CreateArgumentCallSites(Type serviceType, Type implementationType, CallSiteChain callSiteChain, ParameterInfo[] parameters, bool throwIfCallSiteNotFound)
	{
		ServiceCallSite[] parameterCallSites = new ServiceCallSite[parameters.Length];
		for (int index = 0; index < parameters.Length; index++)
		{
			Type parameterType = parameters[index].ParameterType;
			ServiceCallSite callSite = GetCallSite(parameterType, callSiteChain);
			if (callSite == null && ParameterDefaultValue.TryGetDefaultValue(parameters[index], out var defaultValue))
			{
				callSite = new ConstantCallSite(parameterType, defaultValue);
			}
			if (callSite == null)
			{
				if (throwIfCallSiteNotFound)
				{
					throw new InvalidOperationException(Resources.FormatCannotResolveService(parameterType, implementationType));
				}
				return null;
			}
			parameterCallSites[index] = callSite;
		}
		return parameterCallSites;
	}

	public void Add(Type type, ServiceCallSite serviceCallSite)
	{
		_callSiteCache[type] = serviceCallSite;
	}
}
