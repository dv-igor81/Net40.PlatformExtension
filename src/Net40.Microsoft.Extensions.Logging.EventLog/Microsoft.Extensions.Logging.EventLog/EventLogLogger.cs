using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Microsoft.Extensions.Logging.EventLog;

internal class EventLogLogger : ILogger
{
	private readonly string _name;

	private readonly EventLogSettings _settings;

	private readonly IExternalScopeProvider _externalScopeProvider;

	private const string ContinuationString = "...";

	private readonly int _beginOrEndMessageSegmentSize;

	private readonly int _intermediateMessageSegmentSize;

	public IEventLog EventLog { get; }

	public EventLogLogger(string name, EventLogSettings settings, IExternalScopeProvider externalScopeProvider)
	{
		_name = name ?? throw new ArgumentNullException("name");
		_settings = settings ?? throw new ArgumentNullException("settings");
		_externalScopeProvider = externalScopeProvider;
		EventLog = settings.EventLog;
		_beginOrEndMessageSegmentSize = EventLog.MaxMessageSize - "...".Length;
		_intermediateMessageSegmentSize = EventLog.MaxMessageSize - 2 * "...".Length;
	}

	public IDisposable BeginScope<TState>(TState state)
	{
		return _externalScopeProvider?.Push(state);
	}

	public bool IsEnabled(LogLevel logLevel)
	{
		return logLevel != LogLevel.None && (_settings.Filter == null || _settings.Filter(_name, logLevel));
	}

	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
	{
		if (!IsEnabled(logLevel))
		{
			return;
		}
		if (formatter == null)
		{
			throw new ArgumentNullException("formatter");
		}
		string message = formatter(state, exception);
		if (string.IsNullOrEmpty(message))
		{
			return;
		}
		StringBuilder builder = new StringBuilder().Append("Category: ").AppendLine(_name).Append("EventId: ")
			.Append(eventId.Id)
			.AppendLine();
		_externalScopeProvider?.ForEachScope(delegate(object scope, StringBuilder sb)
		{
			if (scope is IEnumerable<KeyValuePair<string, object>> enumerable)
			{
				{
					foreach (KeyValuePair<string, object> current in enumerable)
					{
						sb.Append(current.Key).Append(": ").AppendLine(current.Value?.ToString());
					}
					return;
				}
			}
			if (scope != null)
			{
				sb.AppendLine(scope.ToString());
			}
		}, builder);
		builder.AppendLine().AppendLine(message);
		if (exception != null)
		{
			builder.AppendLine().AppendLine("Exception: ").Append(exception)
				.AppendLine();
		}
		WriteMessage(builder.ToString(), GetEventLogEntryType(logLevel), EventLog.DefaultEventId ?? eventId.Id);
	}

	private void WriteMessage(string message, EventLogEntryType eventLogEntryType, int eventId)
	{
		if (message.Length <= EventLog.MaxMessageSize)
		{
			EventLog.WriteEntry(message, eventLogEntryType, eventId, 0);
			return;
		}
		int startIndex = 0;
		string messageSegment = null;
		while (true)
		{
			if (startIndex == 0)
			{
				messageSegment = message.Substring(startIndex, _beginOrEndMessageSegmentSize) + "...";
				startIndex += _beginOrEndMessageSegmentSize;
			}
			else
			{
				if (message.Length - (startIndex + 1) <= _beginOrEndMessageSegmentSize)
				{
					break;
				}
				messageSegment = "..." + message.Substring(startIndex, _intermediateMessageSegmentSize) + "...";
				startIndex += _intermediateMessageSegmentSize;
			}
			EventLog.WriteEntry(messageSegment, eventLogEntryType, eventId, 0);
		}
		messageSegment = "..." + message.Substring(startIndex);
		EventLog.WriteEntry(messageSegment, eventLogEntryType, eventId, 0);
	}

	private EventLogEntryType GetEventLogEntryType(LogLevel level)
	{
		switch (level)
		{
		case LogLevel.Trace:
		case LogLevel.Debug:
		case LogLevel.Information:
			return EventLogEntryType.Information;
		case LogLevel.Warning:
			return EventLogEntryType.Warning;
		case LogLevel.Error:
		case LogLevel.Critical:
			return EventLogEntryType.Error;
		default:
			return EventLogEntryType.Information;
		}
	}
}
