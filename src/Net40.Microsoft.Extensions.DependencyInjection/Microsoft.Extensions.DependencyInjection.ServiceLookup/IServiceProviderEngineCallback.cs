using System;

namespace Microsoft.Extensions.DependencyInjection.ServiceLookup;

internal interface IServiceProviderEngineCallback
{
	void OnCreate(ServiceCallSite callSite);

	void OnResolve(Type serviceType, IServiceScope scope);
}
