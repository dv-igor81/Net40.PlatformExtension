using System;

namespace Microsoft.Extensions.DependencyInjection.ServiceLookup;

internal class ServiceScopeFactoryCallSite : ServiceCallSite
{
	public override Type ServiceType { get; } = typeof(IServiceScopeFactory);


	public override Type ImplementationType { get; } = typeof(ServiceProviderEngine);


	public override CallSiteKind Kind { get; } = CallSiteKind.ServiceScopeFactory;


	public ServiceScopeFactoryCallSite()
		: base(ResultCache.None)
	{
	}
}
