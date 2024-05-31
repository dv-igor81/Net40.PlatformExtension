using System;
using System.Reflection;

namespace Microsoft.Extensions.Logging;

public static class ProviderAliasUtilities
{
	private const string AliasAttibuteTypeFullName = "Microsoft.Extensions.Logging.ProviderAliasAttribute";

	private const string AliasAttibuteAliasProperty = "Alias";

	public static string GetAlias(Type providerType)
	{
		object[] customAttributes = IntrospectionExtensions.GetTypeInfo(providerType).GetCustomAttributes(inherit: false);
		foreach (object attribute in customAttributes)
		{
			if (attribute.GetType().FullName == "Microsoft.Extensions.Logging.ProviderAliasAttribute")
			{
				PropertyInfo valueProperty = attribute.GetType().GetProperty("Alias", BindingFlags.Instance | BindingFlags.Public);
				if (valueProperty != null)
				{
					return PropertyInfoTheraotExtensions.GetValue(valueProperty, attribute) as string;
				}
			}
		}
		return null;
	}
}
