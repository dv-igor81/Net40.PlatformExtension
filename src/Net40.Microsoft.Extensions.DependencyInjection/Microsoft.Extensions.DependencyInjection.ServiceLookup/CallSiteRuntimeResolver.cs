using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace Microsoft.Extensions.DependencyInjection.ServiceLookup;

internal sealed class CallSiteRuntimeResolver : CallSiteVisitor<RuntimeResolverContext, object>
{
	public object Resolve(ServiceCallSite callSite, ServiceProviderEngineScope scope)
	{
		return VisitCallSite(callSite, new RuntimeResolverContext
		{
			Scope = scope
		});
	}

	protected override object VisitDisposeCache(ServiceCallSite transientCallSite, RuntimeResolverContext context)
	{
		return context.Scope.CaptureDisposable(VisitCallSiteMain(transientCallSite, context));
	}

	protected override object VisitConstructor(ConstructorCallSite constructorCallSite, RuntimeResolverContext context)
	{
		object[] parameterValues;
		if (constructorCallSite.ParameterCallSites.Length == 0)
		{
			parameterValues = ArrayEx.Empty<object>();
		}
		else
		{
			parameterValues = new object[constructorCallSite.ParameterCallSites.Length];
			for (int index = 0; index < parameterValues.Length; index++)
			{
				parameterValues[index] = VisitCallSite(constructorCallSite.ParameterCallSites[index], context);
			}
		}
		try
		{
			return constructorCallSite.ConstructorInfo.Invoke(parameterValues);
		}
		catch (Exception ex) when (ex.InnerException != null)
		{
			ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
			throw;
		}
	}

	protected override object VisitRootCache(ServiceCallSite singletonCallSite, RuntimeResolverContext context)
	{
		return VisitCache(singletonCallSite, context, context.Scope.Engine.Root, RuntimeResolverLock.Root);
	}

	protected override object VisitScopeCache(ServiceCallSite singletonCallSite, RuntimeResolverContext context)
	{
		RuntimeResolverLock requiredScope = ((context.Scope != context.Scope.Engine.Root) ? RuntimeResolverLock.Scope : RuntimeResolverLock.Root);
		return VisitCache(singletonCallSite, context, context.Scope, requiredScope);
	}

	private object VisitCache(ServiceCallSite callSite, RuntimeResolverContext context, ServiceProviderEngineScope serviceProviderEngine, RuntimeResolverLock lockType)
	{
		bool lockTaken = false;
		Dictionary<ServiceCacheKey, object> resolvedServices = serviceProviderEngine.ResolvedServices;
		if ((context.AcquiredLocks & lockType) == 0)
		{
			Monitor.Enter(resolvedServices, ref lockTaken);
		}
		try
		{
			if (!resolvedServices.TryGetValue(callSite.Cache.Key, out var resolved))
			{
				resolved = VisitCallSiteMain(callSite, new RuntimeResolverContext
				{
					Scope = serviceProviderEngine,
					AcquiredLocks = (context.AcquiredLocks | lockType)
				});
				serviceProviderEngine.CaptureDisposable(resolved);
				resolvedServices.Add(callSite.Cache.Key, resolved);
			}
			return resolved;
		}
		finally
		{
			if (lockTaken)
			{
				Monitor.Exit(resolvedServices);
			}
		}
	}

	protected override object VisitConstant(ConstantCallSite constantCallSite, RuntimeResolverContext context)
	{
		return constantCallSite.DefaultValue;
	}

	protected override object VisitServiceProvider(ServiceProviderCallSite serviceProviderCallSite, RuntimeResolverContext context)
	{
		return context.Scope;
	}

	protected override object VisitServiceScopeFactory(ServiceScopeFactoryCallSite serviceScopeFactoryCallSite, RuntimeResolverContext context)
	{
		return context.Scope.Engine;
	}

	protected override object VisitIEnumerable(IEnumerableCallSite enumerableCallSite, RuntimeResolverContext context)
	{
		Array array = Array.CreateInstance(enumerableCallSite.ItemType, enumerableCallSite.ServiceCallSites.Length);
		for (int index = 0; index < enumerableCallSite.ServiceCallSites.Length; index++)
		{
			object value = VisitCallSite(enumerableCallSite.ServiceCallSites[index], context);
			array.SetValue(value, index);
		}
		return array;
	}

	protected override object VisitFactory(FactoryCallSite factoryCallSite, RuntimeResolverContext context)
	{
		return factoryCallSite.Factory(context.Scope);
	}
}
