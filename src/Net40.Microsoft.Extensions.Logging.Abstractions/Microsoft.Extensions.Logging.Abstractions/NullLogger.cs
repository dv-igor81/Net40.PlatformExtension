using System;

namespace Microsoft.Extensions.Logging.Abstractions;

public class NullLogger : ILogger
{
	public static NullLogger Instance { get; } = new NullLogger();


	private NullLogger()
	{
	}

	public IDisposable BeginScope<TState>(TState state)
	{
		return NullScope.Instance;
	}

	public bool IsEnabled(LogLevel logLevel)
	{
		return false;
	}

	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
	{
	}
}
public class NullLogger<T> : ILogger<T>, ILogger
{
	private class NullDisposable : IDisposable
	{
		public static readonly NullDisposable Instance = new NullDisposable();

		public void Dispose()
		{
		}
	}

	public static readonly NullLogger<T> Instance = new NullLogger<T>();

	public IDisposable BeginScope<TState>(TState state)
	{
		return NullDisposable.Instance;
	}

	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
	{
	}

	public bool IsEnabled(LogLevel logLevel)
	{
		return false;
	}
}
