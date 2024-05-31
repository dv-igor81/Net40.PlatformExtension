using System;
using System.Threading;

namespace Microsoft.Extensions.Primitives;

public class CancellationChangeToken : IChangeToken
{
	private class NullDisposable : IDisposable
	{
		public static readonly NullDisposable Instance = new NullDisposable();

		public void Dispose()
		{
		}
	}

	public bool ActiveChangeCallbacks { get; private set; } = true;


	public bool HasChanged => Token.IsCancellationRequested;

	private CancellationToken Token { get; }

	public CancellationChangeToken(CancellationToken cancellationToken)
	{
		Token = cancellationToken;
	}

	public IDisposable RegisterChangeCallback(Action<object> callback, object state)
	{
		bool flag = false;
		if (!ExecutionContext.IsFlowSuppressed())
		{
			ExecutionContext.SuppressFlow();
			flag = true;
		}
		try
		{
			return Token.Register(callback, state);
		}
		catch (ObjectDisposedException)
		{
			ActiveChangeCallbacks = false;
		}
		finally
		{
			if (flag)
			{
				ExecutionContext.RestoreFlow();
			}
		}
		return NullDisposable.Instance;
	}
}
