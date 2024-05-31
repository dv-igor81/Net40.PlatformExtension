using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.Configuration;

internal static class InternalConfigurationRootExtensions
{
	internal static IEnumerable<IConfigurationSection> GetChildrenImplementation(this IConfigurationRoot root, string path)
	{
		return from key in root.Providers.Aggregate(Enumerable.Empty<string>(), (IEnumerable<string> seed, IConfigurationProvider source) => source.GetChildKeys(seed, path)).Distinct(StringComparer.OrdinalIgnoreCase)
			select root.GetSection((path == null) ? key : ConfigurationPath.Combine(path, key));
	}
}
