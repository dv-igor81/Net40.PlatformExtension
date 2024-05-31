using System.Collections.Generic;

namespace Microsoft.Extensions.Logging;

public class LoggerFilterOptions
{
	public bool CaptureScopes { get; set; } = true;


	public LogLevel MinLevel { get; set; }

	public IList<LoggerFilterRule> Rules { get; } = new List<LoggerFilterRule>();

}
