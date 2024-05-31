using System;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging;

internal class StaticFilterOptionsMonitor : IOptionsMonitor<LoggerFilterOptions>
{
	public LoggerFilterOptions CurrentValue { get; }

	public StaticFilterOptionsMonitor(LoggerFilterOptions currentValue)
	{
		CurrentValue = currentValue;
	}

	public IDisposable OnChange(Action<LoggerFilterOptions, string> listener)
	{
		return null;
	}

	public LoggerFilterOptions Get(string name)
	{
		return CurrentValue;
	}
}
