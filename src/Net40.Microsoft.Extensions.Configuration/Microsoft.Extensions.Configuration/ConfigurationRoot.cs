using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Configuration;

public class ConfigurationRoot : IConfigurationRoot, IConfiguration, IDisposable
{
	private readonly IList<IConfigurationProvider> _providers;

	private readonly IList<IDisposable> _changeTokenRegistrations;

	private ConfigurationReloadToken _changeToken = new ConfigurationReloadToken();

	public IEnumerable<IConfigurationProvider> Providers => _providers;

	public string this[string key]
	{
		get
		{
			for (int i = _providers.Count - 1; i >= 0; i--)
			{
				IConfigurationProvider provider = _providers[i];
				if (provider.TryGet(key, out var value))
				{
					return value;
				}
			}
			return null;
		}
		set
		{
			if (!_providers.Any())
			{
				throw new InvalidOperationException("Resources.Error_NoSources");
			}
			foreach (IConfigurationProvider provider in _providers)
			{
				provider.Set(key, value);
			}
		}
	}

	public ConfigurationRoot(IList<IConfigurationProvider> providers)
	{
		if (providers == null)
		{
			throw new ArgumentNullException("providers");
		}
		_providers = providers;
		_changeTokenRegistrations = new List<IDisposable>(providers.Count);
		foreach (IConfigurationProvider p in providers)
		{
			p.Load();
			_changeTokenRegistrations.Add(ChangeToken.OnChange(() => p.GetReloadToken(), delegate
			{
				RaiseChanged();
			}));
		}
	}

	public IEnumerable<IConfigurationSection> GetChildren()
	{
		return this.GetChildrenImplementation(null);
	}

	public IChangeToken GetReloadToken()
	{
		return _changeToken;
	}

	public IConfigurationSection GetSection(string key)
	{
		return new ConfigurationSection(this, key);
	}

	public void Reload()
	{
		foreach (IConfigurationProvider provider in _providers)
		{
			provider.Load();
		}
		RaiseChanged();
	}

	private void RaiseChanged()
	{
		ConfigurationReloadToken previousToken = Interlocked.Exchange(ref _changeToken, new ConfigurationReloadToken());
		previousToken.OnReload();
	}

	public void Dispose()
	{
		foreach (IDisposable registration in _changeTokenRegistrations)
		{
			registration.Dispose();
		}
		foreach (IConfigurationProvider provider in _providers)
		{
			(provider as IDisposable)?.Dispose();
		}
	}
}
