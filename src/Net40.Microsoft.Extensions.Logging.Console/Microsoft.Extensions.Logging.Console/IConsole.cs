using System;

namespace Microsoft.Extensions.Logging.Console;

internal interface IConsole
{
	void Write(string message, ConsoleColor? background, ConsoleColor? foreground);

	void WriteLine(string message, ConsoleColor? background, ConsoleColor? foreground);

	void Flush();
}
