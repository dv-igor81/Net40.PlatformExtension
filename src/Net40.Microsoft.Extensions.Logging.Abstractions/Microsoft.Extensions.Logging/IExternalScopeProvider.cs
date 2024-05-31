using System;

namespace Microsoft.Extensions.Logging;

public interface IExternalScopeProvider
{
	void ForEachScope<TState>(Action<object, TState> callback, TState state);

	IDisposable Push(object state);
}
