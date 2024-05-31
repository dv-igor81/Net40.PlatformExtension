using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging;

public static class LoggingBuilderExtensions
{
	public static ILoggingBuilder AddConfiguration(this ILoggingBuilder builder, IConfiguration configuration)
	{
		builder.AddConfiguration();
		builder.Services.AddSingleton((IConfigureOptions<LoggerFilterOptions>)new LoggerFilterConfigureOptions(configuration));
		builder.Services.AddSingleton((IOptionsChangeTokenSource<LoggerFilterOptions>)new ConfigurationChangeTokenSource<LoggerFilterOptions>(configuration));
		builder.Services.AddSingleton(new LoggingConfiguration(configuration));
		return builder;
	}
}
