using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging;

internal class LoggerFilterConfigureOptions : IConfigureOptions<LoggerFilterOptions>
{
	private const string LogLevelKey = "LogLevel";

	private const string DefaultCategory = "Default";

	private readonly IConfiguration _configuration;

	public LoggerFilterConfigureOptions(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public void Configure(LoggerFilterOptions options)
	{
		LoadDefaultConfigValues(options);
	}

	private void LoadDefaultConfigValues(LoggerFilterOptions options)
	{
		if (_configuration == null)
		{
			return;
		}
		options.CaptureScopes = _configuration.GetValue("CaptureScopes", options.CaptureScopes);
		foreach (IConfigurationSection configurationSection in _configuration.GetChildren())
		{
			if (configurationSection.Key.Equals("LogLevel", StringComparison.OrdinalIgnoreCase))
			{
				LoadRules(options, configurationSection, null);
				continue;
			}
			IConfigurationSection logLevelSection = configurationSection.GetSection("LogLevel");
			if (logLevelSection != null)
			{
				string logger = configurationSection.Key;
				LoadRules(options, logLevelSection, logger);
			}
		}
	}

	private void LoadRules(LoggerFilterOptions options, IConfigurationSection configurationSection, string logger)
	{
		foreach (KeyValuePair<string, string> section in configurationSection.AsEnumerable(makePathsRelative: true))
		{
			if (TryGetSwitch(section.Value, out var level))
			{
				string category = section.Key;
				if (category.Equals("Default", StringComparison.OrdinalIgnoreCase))
				{
					category = null;
				}
				LoggerFilterRule newRule = new LoggerFilterRule(logger, category, level, null);
				options.Rules.Add(newRule);
			}
		}
	}

	private static bool TryGetSwitch(string value, out LogLevel level)
	{
		if (string.IsNullOrEmpty(value))
		{
			level = LogLevel.None;
			return false;
		}
		if (Enum.TryParse<LogLevel>(value, ignoreCase: true, out level))
		{
			return true;
		}
		throw new InvalidOperationException("Configuration value '" + value + "' is not supported.");
	}
}
