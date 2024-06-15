using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Logging.EventSource;

[System.Diagnostics.Tracing.EventSource(Name = "Microsoft-Extensions-Logging")]
public sealed class LoggingEventSource : System.Diagnostics.Tracing.EventSource
{
	public static class Keywords
	{
		public const EventKeywords Meta = (EventKeywords)1L;

		public const EventKeywords Message = (EventKeywords)2L;

		public const EventKeywords FormattedMessage = (EventKeywords)4L;

		public const EventKeywords JsonMessage = (EventKeywords)8L;
	}

	internal static readonly LoggingEventSource Instance = new LoggingEventSource();

	private LoggerFilterRule[] _filterSpec = new LoggerFilterRule[0];

	private CancellationTokenSource _cancellationTokenSource;

	private LoggingEventSource()
		: base(EventSourceSettings.EtwSelfDescribingEventFormat)
	{
	}

	[Event(1, Keywords = (EventKeywords)4L, Level = EventLevel.LogAlways)]
	internal unsafe void FormattedMessage(LogLevel Level, int FactoryID, string LoggerName, int EventId, string EventName, string FormattedMessage)
	{
		if (!IsEnabled())
		{
			return;
		}
		fixed (char* loggerName = LoggerName)
		{
			fixed (char* eventName = EventName)
			{
				fixed (char* formattedMessage = FormattedMessage)
				{
					EventData* eventData = stackalloc EventData[6];
					SetEventData(ref *eventData, ref Level, null);
					SetEventData(ref eventData[1], ref FactoryID, null);
					SetEventData(ref eventData[2], ref LoggerName, loggerName);
					SetEventData(ref eventData[3], ref EventId, null);
					SetEventData(ref eventData[4], ref EventName, eventName);
					SetEventData(ref eventData[5], ref FormattedMessage, formattedMessage);
					WriteEventCore(1, 6, eventData);
				}
			}
		}
	}

	[Event(2, Keywords = (EventKeywords)2L, Level = EventLevel.LogAlways)]
	internal void Message(LogLevel Level, int FactoryID, string LoggerName, int EventId, string EventName, ExceptionInfo Exception, IEnumerable<KeyValuePair<string, string>> Arguments)
	{
		if (IsEnabled())
		{
			WriteEvent(2, Level, FactoryID, LoggerName, EventId, EventName, Exception, Arguments);
		}
	}

	[Event(3, Keywords = (EventKeywords)6L, Level = EventLevel.LogAlways, ActivityOptions = EventActivityOptions.Recursive)]
	internal void ActivityStart(int ID, int FactoryID, string LoggerName, IEnumerable<KeyValuePair<string, string>> Arguments)
	{
		if (IsEnabled())
		{
			WriteEvent(3, ID, FactoryID, LoggerName, Arguments);
		}
	}

	[Event(4, Keywords = (EventKeywords)6L, Level = EventLevel.LogAlways)]
	internal unsafe void ActivityStop(int ID, int FactoryID, string LoggerName)
	{
		if (IsEnabled())
		{
			fixed (char* loggerName = LoggerName)
			{
				EventData* eventData = stackalloc EventData[3];
				SetEventData(ref *eventData, ref ID, null);
				SetEventData(ref eventData[1], ref FactoryID, null);
				SetEventData(ref eventData[2], ref LoggerName, loggerName);
				WriteEventCore(4, 3, eventData);
			}
		}
	}

	[Event(5, Keywords = (EventKeywords)8L, Level = EventLevel.LogAlways)]
	internal unsafe void MessageJson(LogLevel Level, int FactoryID, string LoggerName, int EventId, string EventName, string ExceptionJson, string ArgumentsJson)
	{
		if (!IsEnabled())
		{
			return;
		}
		fixed (char* loggerName = LoggerName)
		{
			fixed (char* eventName = EventName)
			{
				fixed (char* exceptionJson = ExceptionJson)
				{
					fixed (char* argumentsJson = ArgumentsJson)
					{
						EventData* eventData = stackalloc EventData[7];
						SetEventData(ref *eventData, ref Level, null);
						SetEventData(ref eventData[1], ref FactoryID, null);
						SetEventData(ref eventData[2], ref LoggerName, loggerName);
						SetEventData(ref eventData[3], ref EventId, null);
						SetEventData(ref eventData[4], ref EventName, eventName);
						SetEventData(ref eventData[5], ref ExceptionJson, exceptionJson);
						SetEventData(ref eventData[6], ref ArgumentsJson, argumentsJson);
						WriteEventCore(5, 7, eventData);
					}
				}
			}
		}
	}

	[Event(6, Keywords = (EventKeywords)12L, Level = EventLevel.LogAlways, ActivityOptions = EventActivityOptions.Recursive)]
	internal unsafe void ActivityJsonStart(int ID, int FactoryID, string LoggerName, string ArgumentsJson)
	{
		if (!IsEnabled())
		{
			return;
		}
		fixed (char* loggerName = LoggerName)
		{
			fixed (char* argumentsJson = ArgumentsJson)
			{
				EventData* eventData = stackalloc EventData[4];
				SetEventData(ref *eventData, ref ID, null);
				SetEventData(ref eventData[1], ref FactoryID, null);
				SetEventData(ref eventData[2], ref LoggerName, loggerName);
				SetEventData(ref eventData[3], ref ArgumentsJson, argumentsJson);
				WriteEventCore(6, 4, eventData);
			}
		}
	}

	[Event(7, Keywords = (EventKeywords)12L, Level = EventLevel.LogAlways)]
	internal unsafe void ActivityJsonStop(int ID, int FactoryID, string LoggerName)
	{
		if (IsEnabled())
		{
			fixed (char* loggerName = LoggerName)
			{
				EventData* eventData = stackalloc EventData[3];
				SetEventData(ref *eventData, ref ID, null);
				SetEventData(ref eventData[1], ref FactoryID, null);
				SetEventData(ref eventData[2], ref LoggerName, loggerName);
				WriteEventCore(7, 3, eventData);
			}
		}
	}

	protected override void OnEventCommand(EventCommandEventArgs command)
	{
		if (command.Command == EventCommand.Update || command.Command == EventCommand.Enable)
		{
			if (!command.Arguments.TryGetValue("FilterSpecs", out string filterSpec))
			{
				filterSpec = string.Empty;
			}
			SetFilterSpec(filterSpec);
		}
		else if (command.Command == EventCommand.Disable)
		{
			SetFilterSpec(null);
		}
	}

	[NonEvent]
	private void SetFilterSpec(string filterSpec)
	{
		_filterSpec = ParseFilterSpec(filterSpec, GetDefaultLevel());
		FireChangeToken();
	}

	[NonEvent]
	internal IChangeToken GetFilterChangeToken()
	{
		CancellationTokenSource cts = LazyInitializer.EnsureInitialized(ref _cancellationTokenSource, () => new CancellationTokenSource());
		return new CancellationChangeToken(cts.Token);
	}

	[NonEvent]
	private void FireChangeToken()
	{
		Interlocked.Exchange(ref _cancellationTokenSource, null)?.Cancel();
	}

	[NonEvent]
	private static LoggerFilterRule[] ParseFilterSpec(string filterSpec, LogLevel defaultLevel)
	{
		if (filterSpec == string.Empty)
		{
			return new LoggerFilterRule[1]
			{
				new LoggerFilterRule(typeof(EventSourceLoggerProvider).FullName, null, defaultLevel, null)
			};
		}
		List<LoggerFilterRule> rules = new List<LoggerFilterRule>();
		rules.Add(new LoggerFilterRule(typeof(EventSourceLoggerProvider).FullName, null, LogLevel.None, null));
		if (filterSpec != null)
		{
			string[] ruleStrings = filterSpec.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			string[] array = ruleStrings;
			foreach (string rule in array)
			{
				LogLevel level = defaultLevel;
				string[] parts = rule.Split(new char[1] { ':' }, 2);
				string loggerName = parts[0];
				if (loggerName.Length != 0)
				{
					if (loggerName[loggerName.Length - 1] == '*')
					{
						loggerName = loggerName.Substring(0, loggerName.Length - 1);
					}
					if (parts.Length != 2 || TryParseLevel(defaultLevel, parts[1], out level))
					{
						rules.Add(new LoggerFilterRule(typeof(EventSourceLoggerProvider).FullName, loggerName, level, null));
					}
				}
			}
		}
		return rules.ToArray();
	}

	[NonEvent]
	private static bool TryParseLevel(LogLevel defaultLevel, string levelString, out LogLevel ret)
	{
		ret = defaultLevel;
		if (levelString.Length == 0)
		{
			ret = defaultLevel;
			return true;
		}
		switch (levelString)
		{
		case "Trace":
			ret = LogLevel.Trace;
			break;
		case "Debug":
			ret = LogLevel.Debug;
			break;
		case "Information":
			ret = LogLevel.Information;
			break;
		case "Warning":
			ret = LogLevel.Warning;
			break;
		case "Error":
			ret = LogLevel.Error;
			break;
		case "Critical":
			ret = LogLevel.Critical;
			break;
		default:
		{
			if (!int.TryParse(levelString, out var level))
			{
				return false;
			}
			if (0 > level || level > 6)
			{
				return false;
			}
			ret = (LogLevel)level;
			break;
		}
		}
		return true;
	}

	[NonEvent]
	private LogLevel GetDefaultLevel()
	{
		EventKeywords allMessageKeywords = (EventKeywords)14L;
		if (IsEnabled(EventLevel.Verbose, allMessageKeywords))
		{
			return LogLevel.Debug;
		}
		if (IsEnabled(EventLevel.Informational, allMessageKeywords))
		{
			return LogLevel.Information;
		}
		if (IsEnabled(EventLevel.Warning, allMessageKeywords))
		{
			return LogLevel.Warning;
		}
		if (IsEnabled(EventLevel.Error, allMessageKeywords))
		{
			return LogLevel.Error;
		}
		return LogLevel.Critical;
	}

	[NonEvent]
	internal LoggerFilterRule[] GetFilterRules()
	{
		return _filterSpec;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	[NonEvent]
	private static unsafe void SetEventData<T>(ref EventData eventData, ref T value, void* pinnedString = null)
	{
		if (typeof(T) == typeof(string))
		{
			string str = value as string;
			fixed (char* rePinnedString = str)
			{
				Debug.Assert(pinnedString == rePinnedString);
			}
			if (pinnedString != null)
			{
				eventData.DataPointer = (IntPtr)pinnedString;
				eventData.Size = checked((str.Length + 1) * 2);
			}
			else
			{
				eventData.DataPointer = IntPtr.Zero;
				eventData.Size = 0;
			}
		}
		else
		{
			eventData.DataPointer = (IntPtr)Unsafe.AsPointer(ref value);
			eventData.Size = Unsafe.SizeOf<T>();
		}
	}
}
