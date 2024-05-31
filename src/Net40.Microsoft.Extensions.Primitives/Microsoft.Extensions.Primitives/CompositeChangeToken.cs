using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Extensions.Primitives;

public class CompositeChangeToken : IChangeToken
{
	private static readonly Action<object> _onChangeDelegate = OnChange;

	private readonly object _callbackLock = new object();

	private CancellationTokenSource _cancellationTokenSource;

	private bool _registeredCallbackProxy;

	private List<IDisposable> _disposables;

	public IReadOnlyList<IChangeToken> ChangeTokens { get; }

	public bool HasChanged
	{
		get
		{
			if (_cancellationTokenSource != null && _cancellationTokenSource.Token.IsCancellationRequested)
			{
				return true;
			}
			for (int i = 0; i < ChangeTokens.Count; i++)
			{
				if (ChangeTokens[i].HasChanged)
				{
					OnChange(this);
					return true;
				}
			}
			return false;
		}
	}

	public bool ActiveChangeCallbacks { get; }

	public CompositeChangeToken(IReadOnlyList<IChangeToken> changeTokens)
	{
		ChangeTokens = changeTokens ?? throw new ArgumentNullException("changeTokens");
		for (int i = 0; i < ChangeTokens.Count; i++)
		{
			if (ChangeTokens[i].ActiveChangeCallbacks)
			{
				ActiveChangeCallbacks = true;
				break;
			}
		}
	}

	public IDisposable RegisterChangeCallback(Action<object> callback, object state)
	{
		EnsureCallbacksInitialized();
		return _cancellationTokenSource.Token.Register(callback, state);
	}

	private void EnsureCallbacksInitialized()
	{
		if (_registeredCallbackProxy)
		{
			return;
		}
		lock (_callbackLock)
		{
			if (_registeredCallbackProxy)
			{
				return;
			}
			_cancellationTokenSource = new CancellationTokenSource();
			_disposables = new List<IDisposable>();
			for (int i = 0; i < ChangeTokens.Count; i++)
			{
				if (ChangeTokens[i].ActiveChangeCallbacks)
				{
					IDisposable item = ChangeTokens[i].RegisterChangeCallback(_onChangeDelegate, this);
					_disposables.Add(item);
				}
			}
			_registeredCallbackProxy = true;
		}
	}

	private static void OnChange(object state)
	{
		CompositeChangeToken compositeChangeToken = (CompositeChangeToken)state;
		if (compositeChangeToken._cancellationTokenSource == null)
		{
			return;
		}
		lock (compositeChangeToken._callbackLock)
		{
			try
			{
				compositeChangeToken._cancellationTokenSource.Cancel();
			}
			catch
			{
			}
		}
		List<IDisposable> disposables = compositeChangeToken._disposables;
		for (int i = 0; i < disposables.Count; i++)
		{
			disposables[i].Dispose();
		}
	}
}
