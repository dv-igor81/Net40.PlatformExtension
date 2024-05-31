using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection.ServiceLookup;

internal class IEnumerableCallSite : ServiceCallSite
{
	internal Type ItemType { get; }

	internal ServiceCallSite[] ServiceCallSites { get; }

	public override Type ServiceType => typeof(IEnumerable<>).MakeGenericType(ItemType);

	public override Type ImplementationType => ItemType.MakeArrayType();

	public override CallSiteKind Kind { get; } = CallSiteKind.IEnumerable;


	public IEnumerableCallSite(ResultCache cache, Type itemType, ServiceCallSite[] serviceCallSites)
		: base(cache)
	{
		ItemType = itemType;
		ServiceCallSites = serviceCallSites;
	}
}
