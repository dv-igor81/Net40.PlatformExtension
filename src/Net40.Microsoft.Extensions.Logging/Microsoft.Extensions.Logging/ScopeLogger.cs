using System;

namespace Microsoft.Extensions.Logging;

internal readonly struct ScopeLogger
{
	private ILogger Logger { get; }

	private IExternalScopeProvider ExternalScopeProvider { get; }

	public ScopeLogger(ILogger logger, IExternalScopeProvider externalScopeProvider)
	{
		Logger = logger;
		ExternalScopeProvider = externalScopeProvider;
	}

	public IDisposable CreateScope<TState>(TState state)
	{
		if (ExternalScopeProvider != null)
		{
			return ExternalScopeProvider.Push(state);
		}
		return Logger.BeginScope(state);
	}
}
