using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Options;

public class ConfigurationChangeTokenSource<TOptions> : IOptionsChangeTokenSource<TOptions>
{
	private IConfiguration _config;

	public string Name { get; }

	public ConfigurationChangeTokenSource(IConfiguration config)
		: this(Options.DefaultName, config)
	{
	}

	public ConfigurationChangeTokenSource(string name, IConfiguration config)
	{
		if (config == null)
		{
			throw new ArgumentNullException("config");
		}
		_config = config;
		Name = name ?? Options.DefaultName;
	}

	public IChangeToken GetChangeToken()
	{
		return _config.GetReloadToken();
	}
}
