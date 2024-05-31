using System;

namespace Microsoft.Extensions.Logging.Console;

public class ConsoleLoggerOptions
{
	private ConsoleLoggerFormat _format = ConsoleLoggerFormat.Default;

	public bool IncludeScopes { get; set; }

	public bool DisableColors { get; set; }

	public ConsoleLoggerFormat Format
	{
		get
		{
			return _format;
		}
		set
		{
			if (value < ConsoleLoggerFormat.Default || value > ConsoleLoggerFormat.Systemd)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			_format = value;
		}
	}

	public LogLevel LogToStandardErrorThreshold { get; set; } = LogLevel.None;


	public string TimestampFormat { get; set; }
}
