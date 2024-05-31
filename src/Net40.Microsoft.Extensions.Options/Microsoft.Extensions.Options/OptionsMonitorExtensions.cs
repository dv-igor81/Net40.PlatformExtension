using System;

namespace Microsoft.Extensions.Options;

public static class OptionsMonitorExtensions
{
	public static IDisposable OnChange<TOptions>(this IOptionsMonitor<TOptions> monitor, Action<TOptions> listener)
	{
		return monitor.OnChange(delegate(TOptions o, string _)
		{
			listener(o);
		});
	}
}
