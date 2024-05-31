using System;
using Net40.Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection.ServiceLookup;

internal class ConstantCallSite : ServiceCallSite
{
	internal object DefaultValue { get; }

	public override Type ServiceType => DefaultValue.GetType();

	public override Type ImplementationType => DefaultValue.GetType();

	public override CallSiteKind Kind { get; } = CallSiteKind.Constant;


	public ConstantCallSite(Type serviceType, object defaultValue)
		: base(ResultCache.None)
	{
		if (defaultValue != null && !serviceType.IsInstanceOfType(defaultValue))
		{
			throw new ArgumentException(Resources.FormatConstantCantBeConvertedToServiceType(defaultValue.GetType(), serviceType));
		}
		DefaultValue = defaultValue;
	}
}
