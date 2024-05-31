#define DEBUG
using System;
using System.Diagnostics;
using System.Text;

namespace Microsoft.Extensions.Logging.Console;

internal class ConsoleLogger : ILogger
{
	private readonly struct ConsoleColors
	{
		public ConsoleColor? Foreground { get; }

		public ConsoleColor? Background { get; }

		public ConsoleColors(ConsoleColor? foreground, ConsoleColor? background)
		{
			Foreground = foreground;
			Background = background;
		}
	}

	private static readonly string _loglevelPadding;

	private static readonly string _messagePadding;

	private static readonly string _newLineWithMessagePadding;

	private readonly ConsoleColor? DefaultConsoleColor = null;

	private readonly string _name;

	private readonly ConsoleLoggerProcessor _queueProcessor;

	[ThreadStatic]
	private static StringBuilder _logBuilder;

	internal IExternalScopeProvider ScopeProvider { get; set; }

	internal ConsoleLoggerOptions Options { get; set; }

	static ConsoleLogger()
	{
		_loglevelPadding = ": ";
		string logLevelString = GetLogLevelString(LogLevel.Information);
		_messagePadding = new string(' ', logLevelString.Length + _loglevelPadding.Length);
		_newLineWithMessagePadding = Environment.NewLine + _messagePadding;
	}

	internal ConsoleLogger(string name, ConsoleLoggerProcessor loggerProcessor)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		_name = name;
		_queueProcessor = loggerProcessor;
	}

	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
	{
		if (IsEnabled(logLevel))
		{
			if (formatter == null)
			{
				throw new ArgumentNullException("formatter");
			}
			string message = formatter(state, exception);
			if (!string.IsNullOrEmpty(message) || exception != null)
			{
				WriteMessage(logLevel, _name, eventId.Id, message, exception);
			}
		}
	}

	public virtual void WriteMessage(LogLevel logLevel, string logName, int eventId, string message, Exception exception)
	{
		ConsoleLoggerFormat format = Options.Format;
		Debug.Assert(format >= ConsoleLoggerFormat.Default && format <= ConsoleLoggerFormat.Systemd);
		StringBuilder logBuilder = _logBuilder;
		_logBuilder = null;
		if (logBuilder == null)
		{
			logBuilder = new StringBuilder();
		}
		LogMessageEntry entry = format switch
		{
			ConsoleLoggerFormat.Default => CreateDefaultLogMessage(logBuilder, logLevel, logName, eventId, message, exception), 
			ConsoleLoggerFormat.Systemd => CreateSystemdLogMessage(logBuilder, logLevel, logName, eventId, message, exception), 
			_ => default(LogMessageEntry), 
		};
		_queueProcessor.EnqueueMessage(entry);
		logBuilder.Clear();
		if (logBuilder.Capacity > 1024)
		{
			logBuilder.Capacity = 1024;
		}
		_logBuilder = logBuilder;
	}

	private LogMessageEntry CreateDefaultLogMessage(StringBuilder logBuilder, LogLevel logLevel, string logName, int eventId, string message, Exception exception)
	{
		ConsoleColors logLevelColors = GetLogLevelConsoleColors(logLevel);
		string logLevelString = GetLogLevelString(logLevel);
		logBuilder.Append(_loglevelPadding);
		logBuilder.Append(logName);
		logBuilder.Append("[");
		logBuilder.Append(eventId);
		logBuilder.AppendLine("]");
		GetScopeInformation(logBuilder, multiLine: true);
		if (!string.IsNullOrEmpty(message))
		{
			logBuilder.Append(_messagePadding);
			int len = logBuilder.Length;
			logBuilder.AppendLine(message);
			logBuilder.Replace(Environment.NewLine, _newLineWithMessagePadding, len, message.Length);
		}
		if (exception != null)
		{
			logBuilder.AppendLine(exception.ToString());
		}
		string timestampFormat = Options.TimestampFormat;
		return new LogMessageEntry(logBuilder.ToString(), (timestampFormat != null) ? DateTime.Now.ToString(timestampFormat) : null, logLevelString, logLevelColors.Background, logLevelColors.Foreground, DefaultConsoleColor, logLevel >= Options.LogToStandardErrorThreshold);
	}

	private LogMessageEntry CreateSystemdLogMessage(StringBuilder logBuilder, LogLevel logLevel, string logName, int eventId, string message, Exception exception)
	{
		string logLevelString = GetSyslogSeverityString(logLevel);
		logBuilder.Append(logLevelString);
		string timestampFormat = Options.TimestampFormat;
		if (timestampFormat != null)
		{
			logBuilder.Append(DateTime.Now.ToString(timestampFormat));
		}
		logBuilder.Append(logName);
		logBuilder.Append("[");
		logBuilder.Append(eventId);
		logBuilder.Append("]");
		GetScopeInformation(logBuilder, multiLine: false);
		if (!string.IsNullOrEmpty(message))
		{
			logBuilder.Append(' ');
			AppendAndReplaceNewLine(logBuilder, message);
		}
		if (exception != null)
		{
			logBuilder.Append(' ');
			AppendAndReplaceNewLine(logBuilder, exception.ToString());
		}
		logBuilder.Append(Environment.NewLine);
		string message2 = logBuilder.ToString();
		bool logAsError = logLevel >= Options.LogToStandardErrorThreshold;
		return new LogMessageEntry(message2, null, null, null, null, null, logAsError);
		static void AppendAndReplaceNewLine(StringBuilder sb, string message)
		{
			int len = sb.Length;
			sb.Append(message);
			sb.Replace(Environment.NewLine, " ", len, message.Length);
		}
	}

	public bool IsEnabled(LogLevel logLevel)
	{
		return logLevel != LogLevel.None;
	}

	public IDisposable BeginScope<TState>(TState state)
	{
		return ScopeProvider?.Push(state) ?? NullScope.Instance;
	}

	private static string GetLogLevelString(LogLevel logLevel)
	{
		return logLevel switch
		{
			LogLevel.Trace => "trce", 
			LogLevel.Debug => "dbug", 
			LogLevel.Information => "info", 
			LogLevel.Warning => "warn", 
			LogLevel.Error => "fail", 
			LogLevel.Critical => "crit", 
			_ => throw new ArgumentOutOfRangeException("logLevel"), 
		};
	}

	private static string GetSyslogSeverityString(LogLevel logLevel)
	{
		switch (logLevel)
		{
		case LogLevel.Trace:
		case LogLevel.Debug:
			return "<7>";
		case LogLevel.Information:
			return "<6>";
		case LogLevel.Warning:
			return "<4>";
		case LogLevel.Error:
			return "<3>";
		case LogLevel.Critical:
			return "<2>";
		default:
			throw new ArgumentOutOfRangeException("logLevel");
		}
	}

	private ConsoleColors GetLogLevelConsoleColors(LogLevel logLevel)
	{
		if (Options.DisableColors)
		{
			return new ConsoleColors(null, null);
		}
		return logLevel switch
		{
			LogLevel.Critical => new ConsoleColors(ConsoleColor.White, ConsoleColor.Red), 
			LogLevel.Error => new ConsoleColors(ConsoleColor.Black, ConsoleColor.Red), 
			LogLevel.Warning => new ConsoleColors(ConsoleColor.Yellow, ConsoleColor.Black), 
			LogLevel.Information => new ConsoleColors(ConsoleColor.DarkGreen, ConsoleColor.Black), 
			LogLevel.Debug => new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black), 
			LogLevel.Trace => new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black), 
			_ => new ConsoleColors(DefaultConsoleColor, DefaultConsoleColor), 
		};
	}

	private void GetScopeInformation(StringBuilder stringBuilder, bool multiLine)
	{
		IExternalScopeProvider scopeProvider = ScopeProvider;
		if (!Options.IncludeScopes || scopeProvider == null)
		{
			return;
		}
		int initialLength = stringBuilder.Length;
		scopeProvider.ForEachScope(delegate(object scope, (StringBuilder stringBuilder, int) state)
		{
			var (stringBuilder2, num) = state;
			if (num == stringBuilder2.Length)
			{
				stringBuilder2.Append(_messagePadding);
				stringBuilder2.Append("=> ");
			}
			else
			{
				stringBuilder2.Append(" => ");
			}
			stringBuilder2.Append(scope);
		}, (stringBuilder, multiLine ? initialLength : (-1)));
		if (stringBuilder.Length > initialLength && multiLine)
		{
			stringBuilder.AppendLine();
		}
	}
}
