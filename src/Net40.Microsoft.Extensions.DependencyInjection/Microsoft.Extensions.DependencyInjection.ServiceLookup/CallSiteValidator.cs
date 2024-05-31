using System;
using System.Collections.Concurrent;
using Net40.Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection.ServiceLookup;

internal class CallSiteValidator : CallSiteVisitor<CallSiteValidator.CallSiteValidatorState, Type>
{
	internal struct CallSiteValidatorState
	{
		public ServiceCallSite Singleton { get; set; }
	}

	private readonly ConcurrentDictionary<Type, Type> _scopedServices = new ConcurrentDictionary<Type, Type>();

	public void ValidateCallSite(ServiceCallSite callSite)
	{
		Type scoped = VisitCallSite(callSite, default(CallSiteValidatorState));
		if (scoped != null)
		{
			_scopedServices[callSite.ServiceType] = scoped;
		}
	}

	public void ValidateResolution(Type serviceType, IServiceScope scope, IServiceScope rootScope)
	{
		if (scope == rootScope && _scopedServices.TryGetValue(serviceType, out var scopedService))
		{
			if (serviceType == scopedService)
			{
				throw new InvalidOperationException(Resources.FormatDirectScopedResolvedFromRootException(serviceType, "Scoped".ToLowerInvariant()));
			}
			throw new InvalidOperationException(Resources.FormatScopedResolvedFromRootException(serviceType, scopedService, "Scoped".ToLowerInvariant()));
		}
	}

	protected override Type VisitConstructor(ConstructorCallSite constructorCallSite, CallSiteValidatorState state)
	{
		Type result = null;
		ServiceCallSite[] parameterCallSites = constructorCallSite.ParameterCallSites;
		foreach (ServiceCallSite parameterCallSite in parameterCallSites)
		{
			Type scoped = VisitCallSite(parameterCallSite, state);
			if (result == null)
			{
				result = scoped;
			}
		}
		return result;
	}

	protected override Type VisitIEnumerable(IEnumerableCallSite enumerableCallSite, CallSiteValidatorState state)
	{
		Type result = null;
		ServiceCallSite[] serviceCallSites = enumerableCallSite.ServiceCallSites;
		foreach (ServiceCallSite serviceCallSite in serviceCallSites)
		{
			Type scoped = VisitCallSite(serviceCallSite, state);
			if (result == null)
			{
				result = scoped;
			}
		}
		return result;
	}

	protected override Type VisitRootCache(ServiceCallSite singletonCallSite, CallSiteValidatorState state)
	{
		state.Singleton = singletonCallSite;
		return VisitCallSiteMain(singletonCallSite, state);
	}

	protected override Type VisitScopeCache(ServiceCallSite scopedCallSite, CallSiteValidatorState state)
	{
		if (scopedCallSite is ServiceScopeFactoryCallSite)
		{
			return null;
		}
		if (state.Singleton != null)
		{
			throw new InvalidOperationException(Resources.FormatScopedInSingletonException(scopedCallSite.ServiceType, state.Singleton.ServiceType, "Scoped".ToLowerInvariant(), "Singleton".ToLowerInvariant()));
		}
		VisitCallSiteMain(scopedCallSite, state);
		return scopedCallSite.ServiceType;
	}

	protected override Type VisitConstant(ConstantCallSite constantCallSite, CallSiteValidatorState state)
	{
		return null;
	}

	protected override Type VisitServiceProvider(ServiceProviderCallSite serviceProviderCallSite, CallSiteValidatorState state)
	{
		return null;
	}

	protected override Type VisitServiceScopeFactory(ServiceScopeFactoryCallSite serviceScopeFactoryCallSite, CallSiteValidatorState state)
	{
		return null;
	}

	protected override Type VisitFactory(FactoryCallSite factoryCallSite, CallSiteValidatorState state)
	{
		return null;
	}
}
