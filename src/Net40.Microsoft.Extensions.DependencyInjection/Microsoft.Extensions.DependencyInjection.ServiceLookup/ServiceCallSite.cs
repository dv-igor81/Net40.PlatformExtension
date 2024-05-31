using System;

namespace Microsoft.Extensions.DependencyInjection.ServiceLookup;

internal abstract class ServiceCallSite
{
	public abstract Type ServiceType { get; }

	public abstract Type ImplementationType { get; }

	public abstract CallSiteKind Kind { get; }

	public ResultCache Cache { get; }

	public bool CaptureDisposable => ImplementationType == null || typeof(IDisposable).IsAssignableFrom(ImplementationType) || typeof(IAsyncDisposable).IsAssignableFrom(ImplementationType);

	protected ServiceCallSite(ResultCache cache)
	{
		Cache = cache;
	}
}
