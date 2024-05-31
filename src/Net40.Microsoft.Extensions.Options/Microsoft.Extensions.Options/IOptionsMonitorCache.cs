using System;

namespace Microsoft.Extensions.Options;

public interface IOptionsMonitorCache<TOptions> where TOptions : class
{
	TOptions GetOrAdd(string name, Func<TOptions> createOptions);

	bool TryAdd(string name, TOptions options);

	bool TryRemove(string name);

	void Clear();
}
