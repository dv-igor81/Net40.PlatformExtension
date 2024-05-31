namespace System.Threading;

internal static class ExecutionContextEx
{
	internal static object? GetLocalValue(System.Threading.IAsyncLocal local)
	{
		throw new NotImplementedException("ExecutionContextEx.GetLocalValue(IAsyncLocal local)");
	}

	internal static void SetLocalValue(System.Threading.IAsyncLocal local, object newValue, bool needChangeNotifications)
	{
		throw new NotImplementedException("ExecutionContextEx.SetLocalValue(IAsyncLocal local, object newValue, bool needChangeNotifications)");
	}
}
