using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.Console;

namespace Microsoft.Extensions.Logging;

public static class ConsoleLoggerExtensions
{
	public static ILoggingBuilder AddConsole(this ILoggingBuilder builder)
	{
		builder.AddConfiguration();
		builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, ConsoleLoggerProvider>());
		LoggerProviderOptions.RegisterProviderOptions<ConsoleLoggerOptions, ConsoleLoggerProvider>(builder.Services);
		return builder;
	}

	public static ILoggingBuilder AddConsole(this ILoggingBuilder builder, Action<ConsoleLoggerOptions> configure)
	{
		if (configure == null)
		{
			throw new ArgumentNullException("configure");
		}
		builder.AddConsole();
		builder.Services.Configure(configure);
		return builder;
	}
}
