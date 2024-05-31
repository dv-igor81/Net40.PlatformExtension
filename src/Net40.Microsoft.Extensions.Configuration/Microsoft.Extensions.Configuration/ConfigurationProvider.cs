using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Configuration;

public abstract class ConfigurationProvider : IConfigurationProvider
{
	private ConfigurationReloadToken _reloadToken = new ConfigurationReloadToken();

	protected IDictionary<string, string> Data { get; set; }

	protected ConfigurationProvider()
	{
		Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
	}

	public virtual bool TryGet(string key, out string value)
	{
		return Data.TryGetValue(key, out value);
	}

	public virtual void Set(string key, string value)
	{
		Data[key] = value;
	}

	public virtual void Load()
	{
	}

	public virtual IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
	{
		string prefix = ((parentPath == null) ? string.Empty : (parentPath + ConfigurationPath.KeyDelimiter));
		return (from kv in Data
			where kv.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
			select Segment(kv.Key, prefix.Length)).Concat(earlierKeys).OrderBy((string k) => k, ConfigurationKeyComparer.Instance);
	}

	private static string Segment(string key, int prefixLength)
	{
		int indexOf = key.IndexOf(ConfigurationPath.KeyDelimiter, prefixLength, StringComparison.OrdinalIgnoreCase);
		return (indexOf < 0) ? key.Substring(prefixLength) : key.Substring(prefixLength, indexOf - prefixLength);
	}

	public IChangeToken GetReloadToken()
	{
		return _reloadToken;
	}

	protected void OnReload()
	{
		ConfigurationReloadToken previousToken = Interlocked.Exchange(ref _reloadToken, new ConfigurationReloadToken());
		previousToken.OnReload();
	}

	public override string ToString()
	{
		return GetType().Name ?? "";
	}
}
