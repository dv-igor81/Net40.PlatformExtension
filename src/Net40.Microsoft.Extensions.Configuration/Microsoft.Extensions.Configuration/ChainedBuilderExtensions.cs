using System;

namespace Microsoft.Extensions.Configuration;

public static class ChainedBuilderExtensions
{
	public static IConfigurationBuilder AddConfiguration(this IConfigurationBuilder configurationBuilder, IConfiguration config)
	{
		return configurationBuilder.AddConfiguration(config, shouldDisposeConfiguration: false);
	}

	public static IConfigurationBuilder AddConfiguration(this IConfigurationBuilder configurationBuilder, IConfiguration config, bool shouldDisposeConfiguration)
	{
		if (configurationBuilder == null)
		{
			throw new ArgumentNullException("configurationBuilder");
		}
		if (config == null)
		{
			throw new ArgumentNullException("config");
		}
		configurationBuilder.Add(new ChainedConfigurationSource
		{
			Configuration = config,
			ShouldDisposeConfiguration = shouldDisposeConfiguration
		});
		return configurationBuilder;
	}
}
