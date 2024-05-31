using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration.Memory;

namespace Microsoft.Extensions.Configuration;

public static class MemoryConfigurationBuilderExtensions
{
	public static IConfigurationBuilder AddInMemoryCollection(this IConfigurationBuilder configurationBuilder)
	{
		if (configurationBuilder == null)
		{
			throw new ArgumentNullException("configurationBuilder");
		}
		configurationBuilder.Add(new MemoryConfigurationSource());
		return configurationBuilder;
	}

	public static IConfigurationBuilder AddInMemoryCollection(this IConfigurationBuilder configurationBuilder, IEnumerable<KeyValuePair<string, string>> initialData)
	{
		if (configurationBuilder == null)
		{
			throw new ArgumentNullException("configurationBuilder");
		}
		configurationBuilder.Add(new MemoryConfigurationSource
		{
			InitialData = initialData
		});
		return configurationBuilder;
	}
}
