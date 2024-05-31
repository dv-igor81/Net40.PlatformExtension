using System;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.Options;

public class ConfigureFromConfigurationOptions<TOptions> : ConfigureOptions<TOptions> where TOptions : class
{
	public ConfigureFromConfigurationOptions(IConfiguration config)
		: base((Action<TOptions>)delegate(TOptions options)
		{
			config.Bind(options);
		})
	{
		if (config == null)
		{
			throw new ArgumentNullException("config");
		}
	}
}
