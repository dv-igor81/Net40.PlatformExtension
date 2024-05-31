using Microsoft.Extensions.Logging.EventSource;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Logging;

internal class EventLogFiltersConfigureOptionsChangeSource : IOptionsChangeTokenSource<LoggerFilterOptions>
{
	private readonly LoggingEventSource _eventSource;

	public string Name { get; }

	public EventLogFiltersConfigureOptionsChangeSource(LoggingEventSource eventSource)
	{
		_eventSource = eventSource;
	}

	public IChangeToken GetChangeToken()
	{
		return _eventSource.GetFilterChangeToken();
	}
}
