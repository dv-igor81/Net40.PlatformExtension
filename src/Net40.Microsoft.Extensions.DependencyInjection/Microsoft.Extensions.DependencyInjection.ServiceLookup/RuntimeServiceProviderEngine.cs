using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection.ServiceLookup;

internal class RuntimeServiceProviderEngine : ServiceProviderEngine
{
	public RuntimeServiceProviderEngine(IEnumerable<ServiceDescriptor> serviceDescriptors, IServiceProviderEngineCallback callback)
		: base(serviceDescriptors, callback)
	{
	}

	protected override Func<ServiceProviderEngineScope, object> RealizeService(ServiceCallSite callSite)
	{
		return delegate(ServiceProviderEngineScope scope)
		{
			Func<ServiceProviderEngineScope, object> func = (ServiceProviderEngineScope p) => base.RuntimeResolver.Resolve(callSite, p);
			base.RealizedServices[callSite.ServiceType] = func;
			return func(scope);
		};
	}
}
