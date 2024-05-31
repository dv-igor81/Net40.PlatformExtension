using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using Theraot.Collections;

namespace Microsoft.Extensions.Logging.EventSource;

internal class EventSourceLogger : ILogger
{
	private class ActivityScope : IDisposable
	{
		private readonly string _categoryName;

		private readonly int _activityID;

		private readonly int _factoryID;

		private readonly bool _isJsonStop;

		private readonly LoggingEventSource _eventSource;

		public ActivityScope(LoggingEventSource eventSource, string categoryName, int activityID, int factoryID, bool isJsonStop)
		{
			_categoryName = categoryName;
			_activityID = activityID;
			_factoryID = factoryID;
			_isJsonStop = isJsonStop;
			_eventSource = eventSource;
		}

		public void Dispose()
		{
			if (_isJsonStop)
			{
				_eventSource.ActivityJsonStop(_activityID, _factoryID, _categoryName);
			}
			else
			{
				_eventSource.ActivityStop(_activityID, _factoryID, _categoryName);
			}
		}
	}

	private static int _activityIds;

	private readonly LoggingEventSource _eventSource;

	private readonly int _factoryID;

	public string CategoryName { get; }

	public LogLevel Level { get; set; }

	public EventSourceLogger Next { get; }

	public EventSourceLogger(string categoryName, int factoryID, LoggingEventSource eventSource, EventSourceLogger next)
	{
		CategoryName = categoryName;
		Level = LogLevel.Trace;
		_factoryID = factoryID;
		_eventSource = eventSource;
		Next = next;
	}

	public bool IsEnabled(LogLevel logLevel)
	{
		return logLevel != LogLevel.None && logLevel >= Level;
	}

	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
	{
		if (!IsEnabled(logLevel))
		{
			return;
		}
		if (_eventSource.IsEnabled(EventLevel.Critical, (EventKeywords)4L))
		{
			string message = formatter(state, exception);
			_eventSource.FormattedMessage(logLevel, _factoryID, CategoryName, eventId.Id, eventId.Name, message);
		}
		if (_eventSource.IsEnabled(EventLevel.Critical, (EventKeywords)2L))
		{
			ExceptionInfo exceptionInfo2 = GetExceptionInfo(exception);
			IReadOnlyList<KeyValuePair<string, string>> arguments2 = GetProperties(state);
			_eventSource.Message(logLevel, _factoryID, CategoryName, eventId.Id, eventId.Name, exceptionInfo2, arguments2);
		}
		if (_eventSource.IsEnabled(EventLevel.Critical, (EventKeywords)8L))
		{
			string exceptionJson = "{}";
			if (exception != null)
			{
				ExceptionInfo exceptionInfo = GetExceptionInfo(exception);
				KeyValuePair<string, string>[] exceptionInfoData = new KeyValuePair<string, string>[4]
				{
					new KeyValuePair<string, string>("TypeName", exceptionInfo.TypeName),
					new KeyValuePair<string, string>("Message", exceptionInfo.Message),
					new KeyValuePair<string, string>("HResult", exceptionInfo.HResult.ToString()),
					new KeyValuePair<string, string>("VerboseMessage", exceptionInfo.VerboseMessage)
				};
				exceptionJson = ToJson(exceptionInfoData.AsIReadOnlyList());
			}
			IReadOnlyList<KeyValuePair<string, string>> arguments = GetProperties(state);
			_eventSource.MessageJson(logLevel, _factoryID, CategoryName, eventId.Id, eventId.Name, exceptionJson, ToJson(arguments));
		}
	}

	public IDisposable BeginScope<TState>(TState state)
	{
		if (!IsEnabled(LogLevel.Critical))
		{
			return NullScope.Instance;
		}
		int id = Interlocked.Increment(ref _activityIds);
		if (_eventSource.IsEnabled(EventLevel.Critical, (EventKeywords)8L))
		{
			IReadOnlyList<KeyValuePair<string, string>> arguments2 = GetProperties(state);
			_eventSource.ActivityJsonStart(id, _factoryID, CategoryName, ToJson(arguments2));
			return new ActivityScope(_eventSource, CategoryName, id, _factoryID, isJsonStop: true);
		}
		if (_eventSource.IsEnabled(EventLevel.Critical, (EventKeywords)2L) || _eventSource.IsEnabled(EventLevel.Critical, (EventKeywords)4L))
		{
			IReadOnlyList<KeyValuePair<string, string>> arguments = GetProperties(state);
			_eventSource.ActivityStart(id, _factoryID, CategoryName, arguments);
			return new ActivityScope(_eventSource, CategoryName, id, _factoryID, isJsonStop: false);
		}
		return NullScope.Instance;
	}

	private ExceptionInfo GetExceptionInfo(Exception exception)
	{
		return (exception != null) ? new ExceptionInfo(exception) : ExceptionInfo.Empty;
	}

	private IReadOnlyList<KeyValuePair<string, string>> GetProperties(object state)
	{
		if (state is IReadOnlyList<KeyValuePair<string, object>> keyValuePairs)
		{
			KeyValuePair<string, string>[] arguments = new KeyValuePair<string, string>[keyValuePairs.Count];
			for (int i = 0; i < keyValuePairs.Count; i++)
			{
				KeyValuePair<string, object> keyValuePair = keyValuePairs[i];
				arguments[i] = new KeyValuePair<string, string>(keyValuePair.Key, keyValuePair.Value?.ToString());
			}
			return arguments.AsIReadOnlyList();
		}
		return ArrayEx.Empty<KeyValuePair<string, string>>().AsIReadOnlyList();
	}

	private string ToJson(IReadOnlyList<KeyValuePair<string, string>> keyValues)
	{
		using MemoryStreamEx stream = new MemoryStreamEx();
		using Utf8JsonWriter writer = new Utf8JsonWriter(stream);
		writer.WriteStartObject();
		foreach (KeyValuePair<string, string> keyValue in keyValues)
		{
			writer.WriteString(keyValue.Key, keyValue.Value);
		}
		writer.WriteEndObject();
		writer.Flush();
		if (!stream.TryGetBuffer(out var buffer))
		{
			buffer = new ArraySegment<byte>(stream.ToArray());
		}
		return Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
	}
}
