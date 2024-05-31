using System;

namespace Microsoft.Extensions.DependencyInjection.ServiceLookup;

internal struct ServiceCacheKey : IEquatable<ServiceCacheKey>
{
	public static ServiceCacheKey Empty { get; } = new ServiceCacheKey(null, 0);


	public Type Type { get; }

	public int Slot { get; }

	public ServiceCacheKey(Type type, int slot)
	{
		Type = type;
		Slot = slot;
	}

	public bool Equals(ServiceCacheKey other)
	{
		return Type == other.Type && Slot == other.Slot;
	}

	public override int GetHashCode()
	{
		return (Type.GetHashCode() * 397) ^ Slot;
	}
}
