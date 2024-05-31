using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class OptionsConfigurationServiceCollectionExtensions
{
	public static IServiceCollection Configure<TOptions>(this IServiceCollection services, IConfiguration config) where TOptions : class
	{
		return services.Configure<TOptions>(Microsoft.Extensions.Options.Options.DefaultName, config);
	}

	public static IServiceCollection Configure<TOptions>(this IServiceCollection services, string name, IConfiguration config) where TOptions : class
	{
		return services.Configure<TOptions>(name, config, delegate
		{
		});
	}

	public static IServiceCollection Configure<TOptions>(this IServiceCollection services, IConfiguration config, Action<BinderOptions> configureBinder) where TOptions : class
	{
		return services.Configure<TOptions>(Microsoft.Extensions.Options.Options.DefaultName, config, configureBinder);
	}

	public static IServiceCollection Configure<TOptions>(this IServiceCollection services, string name, IConfiguration config, Action<BinderOptions> configureBinder) where TOptions : class
	{
		if (services == null)
		{
			throw new ArgumentNullException("services");
		}
		if (config == null)
		{
			throw new ArgumentNullException("config");
		}
		services.AddOptions();
		services.AddSingleton((IOptionsChangeTokenSource<TOptions>)new ConfigurationChangeTokenSource<TOptions>(name, config));
		return services.AddSingleton((IConfigureOptions<TOptions>)new NamedConfigureFromConfigurationOptions<TOptions>(name, config, configureBinder));
	}
}
