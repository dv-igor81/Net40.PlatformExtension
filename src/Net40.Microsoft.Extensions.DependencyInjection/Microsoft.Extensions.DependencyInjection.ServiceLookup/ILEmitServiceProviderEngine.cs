using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection.ServiceLookup;

internal class ILEmitServiceProviderEngine : ServiceProviderEngine
{
	private readonly ILEmitResolverBuilder _expressionResolverBuilder;

	public ILEmitServiceProviderEngine(IEnumerable<ServiceDescriptor> serviceDescriptors, IServiceProviderEngineCallback callback)
		: base(serviceDescriptors, callback)
	{
		_expressionResolverBuilder = new ILEmitResolverBuilder(base.RuntimeResolver, this, base.Root);
	}

	protected override Func<ServiceProviderEngineScope, object> RealizeService(ServiceCallSite callSite)
	{
		Func<ServiceProviderEngineScope, object> realizedService = _expressionResolverBuilder.Build(callSite);
		base.RealizedServices[callSite.ServiceType] = realizedService;
		return realizedService;
	}
}
