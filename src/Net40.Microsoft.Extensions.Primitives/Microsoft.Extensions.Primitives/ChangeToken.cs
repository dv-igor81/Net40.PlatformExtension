using System;
using System.Threading;

namespace Microsoft.Extensions.Primitives;

public static class ChangeToken
{
	private class ChangeTokenRegistration<TState> : IDisposable
	{
		private class NoopDisposable : IDisposable
		{
			public void Dispose()
			{
			}
		}

		private readonly Func<IChangeToken> _changeTokenProducer;

		private readonly Action<TState> _changeTokenConsumer;

		private readonly TState _state;

		private IDisposable _disposable;

		private static readonly NoopDisposable _disposedSentinel = new NoopDisposable();

		public ChangeTokenRegistration(Func<IChangeToken> changeTokenProducer, Action<TState> changeTokenConsumer, TState state)
		{
			_changeTokenProducer = changeTokenProducer;
			_changeTokenConsumer = changeTokenConsumer;
			_state = state;
			IChangeToken token = changeTokenProducer();
			RegisterChangeTokenCallback(token);
		}

		private void OnChangeTokenFired()
		{
			IChangeToken token = _changeTokenProducer();
			try
			{
				_changeTokenConsumer(_state);
			}
			finally
			{
				RegisterChangeTokenCallback(token);
			}
		}

		private void RegisterChangeTokenCallback(IChangeToken token)
		{
			IDisposable disposable = token.RegisterChangeCallback(delegate(object s)
			{
				((ChangeTokenRegistration<TState>)s).OnChangeTokenFired();
			}, this);
			SetDisposable(disposable);
		}

		private void SetDisposable(IDisposable disposable)
		{
			IDisposable disposable2 = Volatile.Read(ref _disposable);
			if (disposable2 == _disposedSentinel)
			{
				disposable.Dispose();
				return;
			}
			IDisposable disposable3 = Interlocked.CompareExchange(ref _disposable, disposable, disposable2);
			if (disposable3 == _disposedSentinel)
			{
				disposable.Dispose();
			}
			else if (disposable3 != disposable2)
			{
				throw new InvalidOperationException("Somebody else set the _disposable field");
			}
		}

		public void Dispose()
		{
			Interlocked.Exchange(ref _disposable, _disposedSentinel).Dispose();
		}
	}

	public static IDisposable OnChange(Func<IChangeToken> changeTokenProducer, Action changeTokenConsumer)
	{
		if (changeTokenProducer == null)
		{
			throw new ArgumentNullException("changeTokenProducer");
		}
		if (changeTokenConsumer == null)
		{
			throw new ArgumentNullException("changeTokenConsumer");
		}
		return new ChangeTokenRegistration<Action>(changeTokenProducer, delegate(Action callback)
		{
			callback();
		}, changeTokenConsumer);
	}

	public static IDisposable OnChange<TState>(Func<IChangeToken> changeTokenProducer, Action<TState> changeTokenConsumer, TState state)
	{
		if (changeTokenProducer == null)
		{
			throw new ArgumentNullException("changeTokenProducer");
		}
		if (changeTokenConsumer == null)
		{
			throw new ArgumentNullException("changeTokenConsumer");
		}
		return new ChangeTokenRegistration<TState>(changeTokenProducer, changeTokenConsumer, state);
	}
}
