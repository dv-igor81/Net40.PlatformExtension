using System;
using System.Diagnostics.Tracing;

namespace Microsoft.Extensions.Logging.EventSource;

[EventData(Name = "ExceptionInfo")]
internal class ExceptionInfo
{
	private sealed class ExceptionWrapper : Exception
	{
		public int GetHResult()
		{
			return base.HResult;
		}
	}

	public static ExceptionInfo Empty { get; } = new ExceptionInfo();


	public string TypeName { get; }

	public string Message { get; }

	public int HResult { get; }

	public string VerboseMessage { get; }

	private ExceptionInfo()
	{
	}

	public ExceptionInfo(Exception exception)
	{
		TypeName = exception.GetType().FullName;
		Message = exception.Message;
		HResult = ((ExceptionWrapper)exception).GetHResult();
		VerboseMessage = exception.ToString();
	}
}
