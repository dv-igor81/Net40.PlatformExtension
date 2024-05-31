#define DEBUG
using System;
using System.Diagnostics;

namespace Microsoft.Extensions.Logging.Debug;

internal class DebugLogger : ILogger
{
	private readonly string _name;

	public DebugLogger(string name)
	{
		_name = name;
	}

	public IDisposable BeginScope<TState>(TState state)
	{
		return NullScope.Instance;
	}

	public bool IsEnabled(LogLevel logLevel)
	{
		return Debugger.IsAttached && logLevel != LogLevel.None;
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
		if (!string.IsNullOrEmpty(message))
		{
			message = $"{logLevel}: {message}";
			if (exception != null)
			{
				message = message + Environment.NewLine + Environment.NewLine + exception;
			}
			DebugWriteLine(message, _name);
		}
	}

	private void DebugWriteLine(string message, string name)
	{
		System.Diagnostics.Debug.WriteLine(message, name);
	}
}
