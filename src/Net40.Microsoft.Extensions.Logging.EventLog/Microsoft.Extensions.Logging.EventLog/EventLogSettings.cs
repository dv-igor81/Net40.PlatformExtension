using System;

namespace Microsoft.Extensions.Logging.EventLog;

public class EventLogSettings
{
	private IEventLog _eventLog;

	public string LogName { get; set; }

	public string SourceName { get; set; }

	public string MachineName { get; set; }

	public Func<string, LogLevel, bool> Filter { get; set; }

	internal IEventLog EventLog
	{
		get
		{
			return _eventLog ?? (_eventLog = CreateDefaultEventLog());
		}
		set
		{
			_eventLog = value;
		}
	}

	private IEventLog CreateDefaultEventLog()
	{
		string logName = (string.IsNullOrEmpty(LogName) ? "Application" : LogName);
		string machineName = (string.IsNullOrEmpty(MachineName) ? "." : MachineName);
		string sourceName = (string.IsNullOrEmpty(SourceName) ? ".NET Runtime" : SourceName);
		int? defaultEventId = null;
		if (string.IsNullOrEmpty(SourceName))
		{
			sourceName = ".NET Runtime";
			defaultEventId = 1000;
		}
		return new WindowsEventLog(logName, machineName, sourceName)
		{
			DefaultEventId = defaultEventId
		};
	}
}
