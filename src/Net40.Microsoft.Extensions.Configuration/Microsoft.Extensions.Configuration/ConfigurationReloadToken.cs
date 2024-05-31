using System;
using System.Threading;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Configuration;

public class ConfigurationReloadToken : IChangeToken
{
	private CancellationTokenSource _cts = new CancellationTokenSource();

	public bool ActiveChangeCallbacks => true;

	public bool HasChanged => _cts.IsCancellationRequested;

	public IDisposable RegisterChangeCallback(Action<object> callback, object state)
	{
		return _cts.Token.Register(callback, state);
	}

	public void OnReload()
	{
		_cts.Cancel();
	}
}
