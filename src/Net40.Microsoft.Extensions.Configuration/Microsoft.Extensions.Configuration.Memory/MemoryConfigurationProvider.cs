using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Extensions.Configuration.Memory;

public class MemoryConfigurationProvider : ConfigurationProvider, IEnumerable<KeyValuePair<string, string>>, IEnumerable
{
	private readonly MemoryConfigurationSource _source;

	public MemoryConfigurationProvider(MemoryConfigurationSource source)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		_source = source;
		if (_source.InitialData == null)
		{
			return;
		}
		foreach (KeyValuePair<string, string> pair in _source.InitialData)
		{
			base.Data.Add(pair.Key, pair.Value);
		}
	}

	public void Add(string key, string value)
	{
		base.Data.Add(key, value);
	}

	public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
	{
		return base.Data.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
