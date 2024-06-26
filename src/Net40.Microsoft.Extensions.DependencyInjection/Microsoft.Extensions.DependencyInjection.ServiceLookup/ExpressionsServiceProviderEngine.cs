using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection.ServiceLookup;

internal class ExpressionsServiceProviderEngine : ServiceProviderEngine
{
	private readonly ExpressionResolverBuilder _expressionResolverBuilder;

	public ExpressionsServiceProviderEngine(IEnumerable<ServiceDescriptor> serviceDescriptors, IServiceProviderEngineCallback callback)
		: base(serviceDescriptors, callback)
	{
		_expressionResolverBuilder = new ExpressionResolverBuilder(base.RuntimeResolver, this, base.Root);
	}

	protected override Func<ServiceProviderEngineScope, object> RealizeService(ServiceCallSite callSite)
	{
		Func<ServiceProviderEngineScope, object> realizedService = _expressionResolverBuilder.Build(callSite);
		base.RealizedServices[callSite.ServiceType] = realizedService;
		return realizedService;
	}
}
