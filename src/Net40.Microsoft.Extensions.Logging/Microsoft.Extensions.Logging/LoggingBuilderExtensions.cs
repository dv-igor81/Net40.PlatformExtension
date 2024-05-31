using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging;

public static class LoggingBuilderExtensions
{
	public static ILoggingBuilder SetMinimumLevel(this ILoggingBuilder builder, LogLevel level)
	{
		builder.Services.Add(ServiceDescriptor.Singleton((IConfigureOptions<LoggerFilterOptions>)new DefaultLoggerLevelConfigureOptions(level)));
		return builder;
	}

	public static ILoggingBuilder AddProvider(this ILoggingBuilder builder, ILoggerProvider provider)
	{
		builder.Services.AddSingleton(provider);
		return builder;
	}

	public static ILoggingBuilder ClearProviders(this ILoggingBuilder builder)
	{
		builder.Services.RemoveAll<ILoggerProvider>();
		return builder;
	}
}
