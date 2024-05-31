using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class LoggingServiceCollectionExtensions
{
	public static IServiceCollection AddLogging(this IServiceCollection services)
	{
		return services.AddLogging(delegate
		{
		});
	}

	public static IServiceCollection AddLogging(this IServiceCollection services, Action<ILoggingBuilder> configure)
	{
		if (services == null)
		{
			throw new ArgumentNullException("services");
		}
		services.AddOptions();
		services.TryAdd(ServiceDescriptor.Singleton<ILoggerFactory, LoggerFactory>());
		services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));
		services.TryAddEnumerable(ServiceDescriptor.Singleton((IConfigureOptions<LoggerFilterOptions>)new DefaultLoggerLevelConfigureOptions(LogLevel.Information)));
		configure(new LoggingBuilder(services));
		return services;
	}
}
