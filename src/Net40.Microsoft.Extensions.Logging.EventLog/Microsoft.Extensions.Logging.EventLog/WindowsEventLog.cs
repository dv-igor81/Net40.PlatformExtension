using System;
using System.Diagnostics;
using System.Security;

namespace Microsoft.Extensions.Logging.EventLog;

internal class WindowsEventLog : IEventLog
{
	private const int MaximumMessageSize = 31839;

	private bool _enabled = true;

	public System.Diagnostics.EventLog DiagnosticsEventLog { get; }

	public int MaxMessageSize => 31839;

	public int? DefaultEventId { get; set; }

	public WindowsEventLog(string logName, string machineName, string sourceName)
	{
		DiagnosticsEventLog = new System.Diagnostics.EventLog(logName, machineName, sourceName);
	}

	public void WriteEntry(string message, EventLogEntryType type, int eventID, short category)
	{
		try
		{
			if (_enabled)
			{
				DiagnosticsEventLog.WriteEvent(new EventInstance(eventID, category, type), message);
			}
		}
		catch (SecurityException sx)
		{
			_enabled = false;
			try
			{
				using System.Diagnostics.EventLog backupLog = new System.Diagnostics.EventLog("Application", ".", "Application");
				backupLog.WriteEvent(new EventInstance(0L, 0, EventLogEntryType.Error), "Unable to log .NET application events. " + sx.Message);
			}
			catch (Exception)
			{
			}
		}
	}
}
