using Microsoft.Extensions.Logging.EventSource;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging;

internal class EventLogFiltersConfigureOptions : IConfigureOptions<LoggerFilterOptions>
{
	private readonly LoggingEventSource _eventSource;

	public EventLogFiltersConfigureOptions(LoggingEventSource eventSource)
	{
		_eventSource = eventSource;
	}

	public void Configure(LoggerFilterOptions options)
	{
		LoggerFilterRule[] filterRules = _eventSource.GetFilterRules();
		foreach (LoggerFilterRule rule in filterRules)
		{
			options.Rules.Add(rule);
		}
	}
}
