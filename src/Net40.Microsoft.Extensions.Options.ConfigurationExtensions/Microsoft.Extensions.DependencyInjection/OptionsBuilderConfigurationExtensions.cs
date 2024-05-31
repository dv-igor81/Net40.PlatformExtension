using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class OptionsBuilderConfigurationExtensions
{
	public static OptionsBuilder<TOptions> Bind<TOptions>(this OptionsBuilder<TOptions> optionsBuilder, IConfiguration config) where TOptions : class
	{
		return optionsBuilder.Bind(config, delegate
		{
		});
	}

	public static OptionsBuilder<TOptions> Bind<TOptions>(this OptionsBuilder<TOptions> optionsBuilder, IConfiguration config, Action<BinderOptions> configureBinder) where TOptions : class
	{
		if (optionsBuilder == null)
		{
			throw new ArgumentNullException("optionsBuilder");
		}
		optionsBuilder.Services.Configure<TOptions>(optionsBuilder.Name, config, configureBinder);
		return optionsBuilder;
	}
}
