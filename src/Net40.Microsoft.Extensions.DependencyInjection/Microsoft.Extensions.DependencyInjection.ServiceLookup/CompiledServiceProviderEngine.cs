using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection.ServiceLookup;

internal abstract class CompiledServiceProviderEngine : ServiceProviderEngine
{
	private ExpressionResolverBuilder ResolverBuilder { get; }

	protected CompiledServiceProviderEngine(IEnumerable<ServiceDescriptor> serviceDescriptors, IServiceProviderEngineCallback callback) : base(serviceDescriptors, callback)
	{
		ResolverBuilder = new ExpressionResolverBuilder(RuntimeResolver, this, Root);
	}

	protected override Func<ServiceProviderEngineScope, object> RealizeService(ServiceCallSite callSite)
	{
		var realizedService = ResolverBuilder.Build(callSite);
		RealizedServices[callSite.ServiceType] = realizedService;
		return realizedService;
	}
}
