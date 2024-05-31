using System;

namespace Microsoft.Extensions.Logging.Console;

internal readonly struct LogMessageEntry
{
	public readonly string TimeStamp;

	public readonly string LevelString;

	public readonly ConsoleColor? LevelBackground;

	public readonly ConsoleColor? LevelForeground;

	public readonly ConsoleColor? MessageColor;

	public readonly string Message;

	public readonly bool LogAsError;

	public LogMessageEntry(string message, string timeStamp = null, string levelString = null, ConsoleColor? levelBackground = null, ConsoleColor? levelForeground = null, ConsoleColor? messageColor = null, bool logAsError = false)
	{
		TimeStamp = timeStamp;
		LevelString = levelString;
		LevelBackground = levelBackground;
		LevelForeground = levelForeground;
		MessageColor = messageColor;
		Message = message;
		LogAsError = logAsError;
	}
}
