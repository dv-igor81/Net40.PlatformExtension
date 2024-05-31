using System;

namespace Microsoft.Extensions.Logging.Debug;

[ProviderAlias("Debug")]
public class DebugLoggerProvider : ILoggerProvider, IDisposable
{
	public ILogger CreateLogger(string name)
	{
		return new DebugLogger(name);
	}

	public void Dispose()
	{
	}
}
