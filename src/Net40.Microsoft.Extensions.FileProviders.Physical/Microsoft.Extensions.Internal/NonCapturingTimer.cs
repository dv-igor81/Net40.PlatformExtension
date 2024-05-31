using System;
using System.Threading;

namespace Microsoft.Extensions.Internal;

internal static class NonCapturingTimer
{
	public static Timer Create(TimerCallback callback, object state, TimeSpan dueTime, TimeSpan period)
	{
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		bool restoreFlow = false;
		try
		{
			if (!ExecutionContext.IsFlowSuppressed())
			{
				ExecutionContext.SuppressFlow();
				restoreFlow = true;
			}
			return new Timer(callback, state, dueTime, period);
		}
		finally
		{
			if (restoreFlow)
			{
				ExecutionContext.RestoreFlow();
			}
		}
	}
}
