using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.Configuration;

public static class ConfigurationExtensions
{
	public static IConfigurationBuilder Add<TSource>(this IConfigurationBuilder builder, Action<TSource> configureSource) where TSource : IConfigurationSource, new()
	{
		TSource val = new TSource();
		configureSource?.Invoke(val);
		return builder.Add(val);
	}

	public static string GetConnectionString(this IConfiguration configuration, string name)
	{
		return configuration?.GetSection("ConnectionStrings")?[name];
	}

	public static IEnumerable<KeyValuePair<string, string>> AsEnumerable(this IConfiguration configuration)
	{
		return configuration.AsEnumerable(makePathsRelative: false);
	}

	public static IEnumerable<KeyValuePair<string, string>> AsEnumerable(this IConfiguration configuration, bool makePathsRelative)
	{
		Stack<IConfiguration> stack = new Stack<IConfiguration>();
		stack.Push(configuration);
		IConfigurationSection configurationSection = configuration as IConfigurationSection;
		int prefixLength = ((makePathsRelative && configurationSection != null) ? (configurationSection.Path.Length + 1) : 0);
		while (stack.Count > 0)
		{
			IConfiguration config = stack.Pop();
			IConfigurationSection configurationSection2 = config as IConfigurationSection;
			if (configurationSection2 != null && (!makePathsRelative || config != configuration))
			{
				yield return new KeyValuePair<string, string>(configurationSection2.Path.Substring(prefixLength), configurationSection2.Value);
			}
			foreach (IConfigurationSection child in config.GetChildren())
			{
				stack.Push(child);
			}
		}
	}

	public static bool Exists(this IConfigurationSection section)
	{
		if (section == null)
		{
			return false;
		}
		if (section.Value == null)
		{
			return section.GetChildren().Any();
		}
		return true;
	}
}
