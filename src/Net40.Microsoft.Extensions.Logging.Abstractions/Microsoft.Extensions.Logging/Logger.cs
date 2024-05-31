using System;
using Microsoft.Extensions.Internal;

namespace Microsoft.Extensions.Logging;

public class Logger<T> : ILogger<T>, ILogger
{
	private readonly ILogger _logger;

	public Logger(ILoggerFactory factory)
	{
		if (factory == null)
		{
			throw new ArgumentNullException("factory");
		}
		_logger = factory.CreateLogger(TypeNameHelper.GetTypeDisplayName(typeof(T), fullName: true, includeGenericParameterNames: false, includeGenericParameters: false, '.'));
	}

	IDisposable ILogger.BeginScope<TState>(TState state)
	{
		return _logger.BeginScope(state);
	}

	bool ILogger.IsEnabled(LogLevel logLevel)
	{
		return _logger.IsEnabled(logLevel);
	}

	void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
	{
		_logger.Log(logLevel, eventId, state, exception, formatter);
	}
}
