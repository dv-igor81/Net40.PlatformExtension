using System;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Logging;

public static class FilterLoggingBuilderExtensions
{
	public static ILoggingBuilder AddFilter(this ILoggingBuilder builder, Func<string, string, LogLevel, bool> filter)
	{
		return builder.ConfigureFilter(delegate(LoggerFilterOptions options)
		{
			options.AddFilter(filter);
		});
	}

	public static ILoggingBuilder AddFilter(this ILoggingBuilder builder, Func<string, LogLevel, bool> categoryLevelFilter)
	{
		return builder.ConfigureFilter(delegate(LoggerFilterOptions options)
		{
			options.AddFilter(categoryLevelFilter);
		});
	}

	public static ILoggingBuilder AddFilter<T>(this ILoggingBuilder builder, Func<string, LogLevel, bool> categoryLevelFilter) where T : ILoggerProvider
	{
		return builder.ConfigureFilter(delegate(LoggerFilterOptions options)
		{
			options.AddFilter<T>(categoryLevelFilter);
		});
	}

	public static ILoggingBuilder AddFilter(this ILoggingBuilder builder, Func<LogLevel, bool> levelFilter)
	{
		return builder.ConfigureFilter(delegate(LoggerFilterOptions options)
		{
			options.AddFilter(levelFilter);
		});
	}

	public static ILoggingBuilder AddFilter<T>(this ILoggingBuilder builder, Func<LogLevel, bool> levelFilter) where T : ILoggerProvider
	{
		return builder.ConfigureFilter(delegate(LoggerFilterOptions options)
		{
			options.AddFilter<T>(levelFilter);
		});
	}

	public static ILoggingBuilder AddFilter(this ILoggingBuilder builder, string category, LogLevel level)
	{
		return builder.ConfigureFilter(delegate(LoggerFilterOptions options)
		{
			options.AddFilter(category, level);
		});
	}

	public static ILoggingBuilder AddFilter<T>(this ILoggingBuilder builder, string category, LogLevel level) where T : ILoggerProvider
	{
		return builder.ConfigureFilter(delegate(LoggerFilterOptions options)
		{
			options.AddFilter<T>(category, level);
		});
	}

	public static ILoggingBuilder AddFilter(this ILoggingBuilder builder, string category, Func<LogLevel, bool> levelFilter)
	{
		return builder.ConfigureFilter(delegate(LoggerFilterOptions options)
		{
			options.AddFilter(category, levelFilter);
		});
	}

	public static ILoggingBuilder AddFilter<T>(this ILoggingBuilder builder, string category, Func<LogLevel, bool> levelFilter) where T : ILoggerProvider
	{
		return builder.ConfigureFilter(delegate(LoggerFilterOptions options)
		{
			options.AddFilter<T>(category, levelFilter);
		});
	}

	public static LoggerFilterOptions AddFilter(this LoggerFilterOptions builder, Func<string, string, LogLevel, bool> filter)
	{
		return AddRule(builder, null, null, null, filter);
	}

	public static LoggerFilterOptions AddFilter(this LoggerFilterOptions builder, Func<string, LogLevel, bool> categoryLevelFilter)
	{
		return AddRule(builder, null, null, null, (string type, string name, LogLevel level) => categoryLevelFilter(name, level));
	}

	public static LoggerFilterOptions AddFilter<T>(this LoggerFilterOptions builder, Func<string, LogLevel, bool> categoryLevelFilter) where T : ILoggerProvider
	{
		return AddRule(builder, typeof(T).FullName, null, null, (string type, string name, LogLevel level) => categoryLevelFilter(name, level));
	}

	public static LoggerFilterOptions AddFilter(this LoggerFilterOptions builder, Func<LogLevel, bool> levelFilter)
	{
		return AddRule(builder, null, null, null, (string type, string name, LogLevel level) => levelFilter(level));
	}

	public static LoggerFilterOptions AddFilter<T>(this LoggerFilterOptions builder, Func<LogLevel, bool> levelFilter) where T : ILoggerProvider
	{
		return AddRule(builder, typeof(T).FullName, null, null, (string type, string name, LogLevel level) => levelFilter(level));
	}

	public static LoggerFilterOptions AddFilter(this LoggerFilterOptions builder, string category, LogLevel level)
	{
		return AddRule(builder, null, category, level);
	}

	public static LoggerFilterOptions AddFilter<T>(this LoggerFilterOptions builder, string category, LogLevel level) where T : ILoggerProvider
	{
		return AddRule(builder, typeof(T).FullName, category, level);
	}

	public static LoggerFilterOptions AddFilter(this LoggerFilterOptions builder, string category, Func<LogLevel, bool> levelFilter)
	{
		return AddRule(builder, null, category, null, (string type, string name, LogLevel level) => levelFilter(level));
	}

	public static LoggerFilterOptions AddFilter<T>(this LoggerFilterOptions builder, string category, Func<LogLevel, bool> levelFilter) where T : ILoggerProvider
	{
		return AddRule(builder, typeof(T).FullName, category, null, (string type, string name, LogLevel level) => levelFilter(level));
	}

	private static ILoggingBuilder ConfigureFilter(this ILoggingBuilder builder, Action<LoggerFilterOptions> configureOptions)
	{
		builder.Services.Configure(configureOptions);
		return builder;
	}

	private static LoggerFilterOptions AddRule(LoggerFilterOptions options, string type = null, string category = null, LogLevel? level = null, Func<string, string, LogLevel, bool> filter = null)
	{
		options.Rules.Add(new LoggerFilterRule(type, category, level, filter));
		return options;
	}
}
