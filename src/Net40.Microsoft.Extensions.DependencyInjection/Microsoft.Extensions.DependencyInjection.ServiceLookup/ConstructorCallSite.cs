using System;
using System.Reflection;
using Net40.Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection.ServiceLookup;

internal class ConstructorCallSite : ServiceCallSite
{
	internal ConstructorInfo ConstructorInfo { get; }

	internal ServiceCallSite[] ParameterCallSites { get; }

	public override Type ServiceType { get; }

	public override Type ImplementationType => ConstructorInfo.DeclaringType;

	public override CallSiteKind Kind { get; } = CallSiteKind.Constructor;


	public ConstructorCallSite(ResultCache cache, Type serviceType, ConstructorInfo constructorInfo)
		: this(cache, serviceType, constructorInfo, ArrayEx.Empty<ServiceCallSite>())
	{
	}

	public ConstructorCallSite(ResultCache cache, Type serviceType, ConstructorInfo constructorInfo, ServiceCallSite[] parameterCallSites)
		: base(cache)
	{
		if (!serviceType.IsAssignableFrom(constructorInfo.DeclaringType))
		{
			throw new ArgumentException(Resources.FormatImplementationTypeCantBeConvertedToServiceType(constructorInfo.DeclaringType, serviceType));
		}
		ServiceType = serviceType;
		ConstructorInfo = constructorInfo;
		ParameterCallSites = parameterCallSites;
	}
}
