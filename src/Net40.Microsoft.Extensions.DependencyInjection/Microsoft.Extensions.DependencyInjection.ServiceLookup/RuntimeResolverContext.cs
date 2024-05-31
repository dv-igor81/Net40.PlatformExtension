namespace Microsoft.Extensions.DependencyInjection.ServiceLookup;

internal struct RuntimeResolverContext
{
	public ServiceProviderEngineScope Scope { get; set; }

	public RuntimeResolverLock AcquiredLocks { get; set; }
}
