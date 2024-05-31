using System;
using System.Text;

namespace Microsoft.Extensions.Logging.Console;

internal class AnsiLogConsole : IConsole
{
	private readonly StringBuilder _outputBuilder;

	private readonly IAnsiSystemConsole _systemConsole;

	public AnsiLogConsole(IAnsiSystemConsole systemConsole)
	{
		_outputBuilder = new StringBuilder();
		_systemConsole = systemConsole;
	}

	public void Write(string message, ConsoleColor? background, ConsoleColor? foreground)
	{
		if (background.HasValue)
		{
			_outputBuilder.Append(GetBackgroundColorEscapeCode(background.Value));
		}
		if (foreground.HasValue)
		{
			_outputBuilder.Append(GetForegroundColorEscapeCode(foreground.Value));
		}
		_outputBuilder.Append(message);
		if (foreground.HasValue)
		{
			_outputBuilder.Append("\u001b[39m\u001b[22m");
		}
		if (background.HasValue)
		{
			_outputBuilder.Append("\u001b[49m");
		}
	}

	public void WriteLine(string message, ConsoleColor? background, ConsoleColor? foreground)
	{
		Write(message, background, foreground);
		_outputBuilder.AppendLine();
	}

	public void Flush()
	{
		_systemConsole.Write(_outputBuilder.ToString());
		_outputBuilder.Clear();
	}

	private static string GetForegroundColorEscapeCode(ConsoleColor color)
	{
		return color switch
		{
			ConsoleColor.Black => "\u001b[30m", 
			ConsoleColor.DarkRed => "\u001b[31m", 
			ConsoleColor.DarkGreen => "\u001b[32m", 
			ConsoleColor.DarkYellow => "\u001b[33m", 
			ConsoleColor.DarkBlue => "\u001b[34m", 
			ConsoleColor.DarkMagenta => "\u001b[35m", 
			ConsoleColor.DarkCyan => "\u001b[36m", 
			ConsoleColor.Gray => "\u001b[37m", 
			ConsoleColor.Red => "\u001b[1m\u001b[31m", 
			ConsoleColor.Green => "\u001b[1m\u001b[32m", 
			ConsoleColor.Yellow => "\u001b[1m\u001b[33m", 
			ConsoleColor.Blue => "\u001b[1m\u001b[34m", 
			ConsoleColor.Magenta => "\u001b[1m\u001b[35m", 
			ConsoleColor.Cyan => "\u001b[1m\u001b[36m", 
			ConsoleColor.White => "\u001b[1m\u001b[37m", 
			_ => "\u001b[39m\u001b[22m", 
		};
	}

	private static string GetBackgroundColorEscapeCode(ConsoleColor color)
	{
		return color switch
		{
			ConsoleColor.Black => "\u001b[40m", 
			ConsoleColor.Red => "\u001b[41m", 
			ConsoleColor.Green => "\u001b[42m", 
			ConsoleColor.Yellow => "\u001b[43m", 
			ConsoleColor.Blue => "\u001b[44m", 
			ConsoleColor.Magenta => "\u001b[45m", 
			ConsoleColor.Cyan => "\u001b[46m", 
			ConsoleColor.White => "\u001b[47m", 
			_ => "\u001b[49m", 
		};
	}
}
