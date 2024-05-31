using System;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging;

internal class DefaultLoggerLevelConfigureOptions : ConfigureOptions<LoggerFilterOptions>
{
	public DefaultLoggerLevelConfigureOptions(LogLevel level)
		: base((Action<LoggerFilterOptions>)delegate(LoggerFilterOptions options)
		{
			options.MinLevel = level;
		})
	{
	}
}
