using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Configuration;

public class ChainedConfigurationProvider : IConfigurationProvider, IDisposable
{
	private readonly IConfiguration _config;

	private readonly bool _shouldDisposeConfig;

	public ChainedConfigurationProvider(ChainedConfigurationSource source)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (source.Configuration == null)
		{
			throw new ArgumentNullException("Configuration");
		}
		_config = source.Configuration;
		_shouldDisposeConfig = source.ShouldDisposeConfiguration;
	}

	public bool TryGet(string key, out string value)
	{
		value = _config[key];
		return !string.IsNullOrEmpty(value);
	}

	public void Set(string key, string value)
	{
		_config[key] = value;
	}

	public IChangeToken GetReloadToken()
	{
		return _config.GetReloadToken();
	}

	public void Load()
	{
	}

	public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
	{
		IConfiguration configuration;
		if (parentPath != null)
		{
			IConfiguration section2 = _config.GetSection(parentPath);
			configuration = section2;
		}
		else
		{
			configuration = _config;
		}
		IConfiguration section = configuration;
		IEnumerable<IConfigurationSection> children = section.GetChildren();
		List<string> keys = new List<string>();
		keys.AddRange(children.Select((IConfigurationSection c) => c.Key));
		return keys.Concat(earlierKeys).OrderBy((string k) => k, ConfigurationKeyComparer.Instance);
	}

	public void Dispose()
	{
		if (_shouldDisposeConfig)
		{
			(_config as IDisposable)?.Dispose();
		}
	}
}
