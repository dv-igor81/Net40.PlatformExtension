using System;
using System.Collections.Concurrent;

namespace Microsoft.Extensions.Options;

public class OptionsCache<TOptions> : IOptionsMonitorCache<TOptions> where TOptions : class
{
	private readonly ConcurrentDictionary<string, Lazy<TOptions>> _cache = new ConcurrentDictionary<string, Lazy<TOptions>>(StringComparer.Ordinal);

	public void Clear()
	{
		_cache.Clear();
	}

	public virtual TOptions GetOrAdd(string name, Func<TOptions> createOptions)
	{
		if (createOptions == null)
		{
			throw new ArgumentNullException("createOptions");
		}
		name = name ?? Options.DefaultName;
		return _cache.GetOrAdd(name, new Lazy<TOptions>(createOptions)).Value;
	}

	public virtual bool TryAdd(string name, TOptions options)
	{
		if (options == null)
		{
			throw new ArgumentNullException("options");
		}
		name = name ?? Options.DefaultName;
		return _cache.TryAdd(name, new Lazy<TOptions>(() => options));
	}

	public virtual bool TryRemove(string name)
	{
		name = name ?? Options.DefaultName;
		Lazy<TOptions> ignored;
		return _cache.TryRemove(name, out ignored);
	}
}
