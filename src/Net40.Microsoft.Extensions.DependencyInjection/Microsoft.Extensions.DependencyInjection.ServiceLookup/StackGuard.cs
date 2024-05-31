using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection.ServiceLookup;

internal sealed class StackGuard
{
	private const int MaxExecutionStackCount = 1024;

	private int _executionStackCount;

	public bool TryEnterOnCurrentStack()
	{
		try
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
			return true;
		}
		catch (InsufficientExecutionStackException)
		{
		}
		if (_executionStackCount < 1024)
		{
			return false;
		}
		throw new InsufficientExecutionStackException();
	}

	public TR RunOnEmptyStack<T1, T2, TR>(Func<T1, T2, TR> action, T1 arg1, T2 arg2)
	{
		return RunOnEmptyStackCore(delegate(object s)
		{
			Tuple<Func<T1, T2, TR>, T1, T2> tuple = (Tuple<Func<T1, T2, TR>, T1, T2>)s;
			return tuple.Item1(tuple.Item2, tuple.Item3);
		}, Tuple.Create(action, arg1, arg2));
	}

	private R RunOnEmptyStackCore<R>(Func<object, R> action, object state)
	{
		_executionStackCount++;
		try
		{
			Task<R> task = Task.Factory.StartNew(action, state, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
			TaskAwaiter<R> awaiter = TaskTheraotExtensions.GetAwaiter(task);
			if (!awaiter.IsCompleted)
			{
				((IAsyncResult)task).AsyncWaitHandle.WaitOne();
			}
			return awaiter.GetResult();
		}
		finally
		{
			_executionStackCount--;
		}
	}
}
