using System;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Hosting.Internal;

internal static class HostingLoggerExtensions
{
	public static void ApplicationError(this ILogger logger, EventId eventId, string message, Exception exception)
	{
		if (exception is ReflectionTypeLoadException { LoaderExceptions: var loaderExceptions })
		{
			foreach (Exception ex in loaderExceptions)
			{
				message = message + Environment.NewLine + ex.Message;
			}
		}
		string message2 = message;
		logger.LogCritical(eventId, exception, message2);
	}

	public static void Starting(this ILogger logger)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			logger.LogDebug(1, "Hosting starting");
		}
	}

	public static void Started(this ILogger logger)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			logger.LogDebug(2, "Hosting started");
		}
	}

	public static void Stopping(this ILogger logger)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			logger.LogDebug(3, "Hosting stopping");
		}
	}

	public static void Stopped(this ILogger logger)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			logger.LogDebug(4, "Hosting stopped");
		}
	}

	public static void StoppedWithException(this ILogger logger, Exception ex)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			logger.LogDebug(5, ex, "Hosting shutdown exception");
		}
	}
}
