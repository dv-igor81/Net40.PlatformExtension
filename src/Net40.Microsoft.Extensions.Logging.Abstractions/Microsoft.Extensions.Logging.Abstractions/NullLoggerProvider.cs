using System;

namespace Microsoft.Extensions.Logging.Abstractions;

public class NullLoggerProvider : ILoggerProvider, IDisposable
{
	public static NullLoggerProvider Instance { get; } = new NullLoggerProvider();


	private NullLoggerProvider()
	{
	}

	public ILogger CreateLogger(string categoryName)
	{
		return NullLogger.Instance;
	}

	public void Dispose()
	{
	}
}
