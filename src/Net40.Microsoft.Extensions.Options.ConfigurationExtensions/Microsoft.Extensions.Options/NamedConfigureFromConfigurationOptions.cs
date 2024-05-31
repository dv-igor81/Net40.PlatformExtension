using System;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.Options;

public class NamedConfigureFromConfigurationOptions<TOptions> : ConfigureNamedOptions<TOptions> where TOptions : class
{
	public NamedConfigureFromConfigurationOptions(string name, IConfiguration config)
		: this(name, config, (Action<BinderOptions>)delegate
		{
		})
	{
	}

	public NamedConfigureFromConfigurationOptions(string name, IConfiguration config, Action<BinderOptions> configureBinder)
		: base(name, (Action<TOptions>)delegate(TOptions options)
		{
			config.Bind(options, configureBinder);
		})
	{
		if (config == null)
		{
			throw new ArgumentNullException("config");
		}
	}
}
