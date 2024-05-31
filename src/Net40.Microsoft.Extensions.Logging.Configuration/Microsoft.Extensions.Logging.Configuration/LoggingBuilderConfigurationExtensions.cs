using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.Logging.Configuration;

public static class LoggingBuilderConfigurationExtensions
{
	public static void AddConfiguration(this ILoggingBuilder builder)
	{
		builder.Services.TryAddSingleton<ILoggerProviderConfigurationFactory, LoggerProviderConfigurationFactory>();
		builder.Services.TryAddSingleton(typeof(ILoggerProviderConfiguration<>), typeof(LoggerProviderConfiguration<>));
	}
}
