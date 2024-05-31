using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.EventSource;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging;

public static class EventSourceLoggerFactoryExtensions
{
	public static ILoggingBuilder AddEventSourceLogger(this ILoggingBuilder builder)
	{
		if (builder == null)
		{
			throw new ArgumentNullException("builder");
		}
		builder.Services.TryAddSingleton(LoggingEventSource.Instance);
		builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, EventSourceLoggerProvider>());
		builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<LoggerFilterOptions>, EventLogFiltersConfigureOptions>());
		builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IOptionsChangeTokenSource<LoggerFilterOptions>, EventLogFiltersConfigureOptionsChangeSource>());
		return builder;
	}
}
