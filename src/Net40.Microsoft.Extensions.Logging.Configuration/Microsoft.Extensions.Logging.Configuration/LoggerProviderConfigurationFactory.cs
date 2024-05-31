using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.Logging.Configuration;

internal class LoggerProviderConfigurationFactory : ILoggerProviderConfigurationFactory
{
	private readonly IEnumerable<LoggingConfiguration> _configurations;

	public LoggerProviderConfigurationFactory(IEnumerable<LoggingConfiguration> configurations)
	{
		_configurations = configurations;
	}

	public IConfiguration GetConfiguration(Type providerType)
	{
		if (providerType == null)
		{
			throw new ArgumentNullException("providerType");
		}
		string fullName = providerType.FullName;
		string alias = ProviderAliasUtilities.GetAlias(providerType);
		ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
		foreach (LoggingConfiguration configuration in _configurations)
		{
			IConfigurationSection sectionFromFullName = configuration.Configuration.GetSection(fullName);
			configurationBuilder.AddConfiguration(sectionFromFullName);
			if (!string.IsNullOrWhiteSpace(alias))
			{
				IConfigurationSection sectionFromAlias = configuration.Configuration.GetSection(alias);
				configurationBuilder.AddConfiguration(sectionFromAlias);
			}
		}
		return configurationBuilder.Build();
	}
}
