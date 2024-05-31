using System;

namespace Microsoft.Extensions.DependencyInjection.ServiceLookup;

internal class FactoryCallSite : ServiceCallSite
{
	public Func<IServiceProvider, object> Factory { get; }

	public override Type ServiceType { get; }

	public override Type ImplementationType => null;

	public override CallSiteKind Kind { get; } = CallSiteKind.Factory;


	public FactoryCallSite(ResultCache cache, Type serviceType, Func<IServiceProvider, object> factory)
		: base(cache)
	{
		Factory = factory;
		ServiceType = serviceType;
	}
}
