using System;

namespace Microsoft.Extensions.Logging;

public interface ILoggerFactory : IDisposable
{
	ILogger CreateLogger(string categoryName);

	void AddProvider(ILoggerProvider provider);
}
