using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.EventLog;

namespace Microsoft.Extensions.Logging;

public static class EventLoggerFactoryExtensions
{
	public static ILoggingBuilder AddEventLog(this ILoggingBuilder builder)
	{
		if (builder == null)
		{
			throw new ArgumentNullException("builder");
		}
		builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, EventLogLoggerProvider>());
		return builder;
	}

	public static ILoggingBuilder AddEventLog(this ILoggingBuilder builder, EventLogSettings settings)
	{
		if (builder == null)
		{
			throw new ArgumentNullException("builder");
		}
		if (settings == null)
		{
			throw new ArgumentNullException("settings");
		}
		builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton((ILoggerProvider)new EventLogLoggerProvider(settings)));
		return builder;
	}

	public static ILoggingBuilder AddEventLog(this ILoggingBuilder builder, Action<EventLogSettings> configure)
	{
		if (configure == null)
		{
			throw new ArgumentNullException("configure");
		}
		builder.AddEventLog();
		builder.Services.Configure(configure);
		return builder;
	}
}
