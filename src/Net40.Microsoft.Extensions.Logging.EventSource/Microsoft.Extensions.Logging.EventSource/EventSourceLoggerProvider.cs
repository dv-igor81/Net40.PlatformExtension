using System;
using System.Threading;

namespace Microsoft.Extensions.Logging.EventSource;

[ProviderAlias("EventSource")]
public class EventSourceLoggerProvider : ILoggerProvider, IDisposable
{
	private static int _globalFactoryID;

	private readonly int _factoryID;

	private EventSourceLogger _loggers;

	private readonly LoggingEventSource _eventSource;

	public EventSourceLoggerProvider(LoggingEventSource eventSource)
	{
		if (eventSource == null)
		{
			throw new ArgumentNullException("eventSource");
		}
		_eventSource = eventSource;
		_factoryID = Interlocked.Increment(ref _globalFactoryID);
	}

	public ILogger CreateLogger(string categoryName)
	{
		return _loggers = new EventSourceLogger(categoryName, _factoryID, _eventSource, _loggers);
	}

	public void Dispose()
	{
		for (EventSourceLogger logger = _loggers; logger != null; logger = logger.Next)
		{
			logger.Level = LogLevel.None;
		}
	}
}
