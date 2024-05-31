using System;

namespace Microsoft.Extensions.Logging;

internal readonly struct MessageLogger
{
	public ILogger Logger { get; }

	public string Category { get; }

	private string ProviderTypeFullName { get; }

	public LogLevel? MinLevel { get; }

	public Func<string, string, LogLevel, bool> Filter { get; }

	public MessageLogger(ILogger logger, string category, string providerTypeFullName, LogLevel? minLevel, Func<string, string, LogLevel, bool> filter)
	{
		Logger = logger;
		Category = category;
		ProviderTypeFullName = providerTypeFullName;
		MinLevel = minLevel;
		Filter = filter;
	}

	public bool IsEnabled(LogLevel level)
	{
		if (MinLevel.HasValue && level < MinLevel)
		{
			return false;
		}
		if (Filter != null)
		{
			return Filter(ProviderTypeFullName, Category, level);
		}
		return true;
	}
}
