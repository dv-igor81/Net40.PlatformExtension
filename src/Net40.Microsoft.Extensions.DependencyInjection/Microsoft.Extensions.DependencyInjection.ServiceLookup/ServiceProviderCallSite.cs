using System;

namespace Microsoft.Extensions.DependencyInjection.ServiceLookup;

internal class ServiceProviderCallSite : ServiceCallSite
{
	public override Type ServiceType { get; } = typeof(IServiceProvider);


	public override Type ImplementationType { get; } = typeof(ServiceProvider);


	public override CallSiteKind Kind { get; } = CallSiteKind.ServiceProvider;


	public ServiceProviderCallSite()
		: base(ResultCache.None)
	{
	}
}
