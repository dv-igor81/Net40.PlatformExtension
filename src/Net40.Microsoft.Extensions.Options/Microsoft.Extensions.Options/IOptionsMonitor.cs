using System;

namespace Microsoft.Extensions.Options;

public interface IOptionsMonitor<out TOptions>
{
	TOptions CurrentValue { get; }

	TOptions Get(string name);

	IDisposable OnChange(Action<TOptions, string> listener);
}
