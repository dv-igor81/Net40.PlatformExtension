using System;

namespace Microsoft.Extensions.Logging;

public class LoggerFilterRule
{
	public string ProviderName { get; }

	public string CategoryName { get; }

	public LogLevel? LogLevel { get; }

	public Func<string, string, LogLevel, bool> Filter { get; }

	public LoggerFilterRule(string providerName, string categoryName, LogLevel? logLevel, Func<string, string, LogLevel, bool> filter)
	{
		ProviderName = providerName;
		CategoryName = categoryName;
		LogLevel = logLevel;
		Filter = filter;
	}

	public override string ToString()
	{
		return string.Format("{0}: '{1}', {2}: '{3}', {4}: '{5}', {6}: '{7}'", "ProviderName", ProviderName, "CategoryName", CategoryName, "LogLevel", LogLevel, "Filter", Filter);
	}
}
