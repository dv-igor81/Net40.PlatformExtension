using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Extensions.Configuration;

public static class ConfigurationRootExtensions
{
	public static string GetDebugView(this IConfigurationRoot root)
	{
		StringBuilder stringBuilder2 = new StringBuilder();
		RecurseChildren(stringBuilder2, root.GetChildren(), "");
		return stringBuilder2.ToString();
		void RecurseChildren(StringBuilder stringBuilder, IEnumerable<IConfigurationSection> children, string indent)
		{
			foreach (IConfigurationSection child in children)
			{
				(string, IConfigurationProvider) valueAndProvider = GetValueAndProvider(root, child.Path);
				if (valueAndProvider.Item2 != null)
				{
					stringBuilder.Append(indent).Append(child.Key).Append("=")
						.Append(valueAndProvider.Item1)
						.Append(" (")
						.Append(valueAndProvider.Item2)
						.AppendLine(")");
				}
				else
				{
					stringBuilder.Append(indent).Append(child.Key).AppendLine(":");
				}
				RecurseChildren(stringBuilder, child.GetChildren(), indent + "  ");
			}
		}
	}

	private static (string Value, IConfigurationProvider Provider) GetValueAndProvider(IConfigurationRoot root, string key)
	{
		foreach (IConfigurationProvider item in root.Providers.Reverse())
		{
			if (item.TryGet(key, out var value))
			{
				return (Value: value, Provider: item);
			}
		}
		return (Value: null, Provider: null);
	}
}
