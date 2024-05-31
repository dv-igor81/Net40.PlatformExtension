using System;

namespace Microsoft.Extensions.Logging;

public interface ILoggerProvider : IDisposable
{
	ILogger CreateLogger(string categoryName);
}
