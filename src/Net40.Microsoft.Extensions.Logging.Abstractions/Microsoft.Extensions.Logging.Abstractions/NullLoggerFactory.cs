using System;

namespace Microsoft.Extensions.Logging.Abstractions;

public class NullLoggerFactory : ILoggerFactory, IDisposable
{
	public static readonly NullLoggerFactory Instance = new NullLoggerFactory();

	public ILogger CreateLogger(string name)
	{
		return NullLogger.Instance;
	}

	public void AddProvider(ILoggerProvider provider)
	{
	}

	public void Dispose()
	{
	}
}
