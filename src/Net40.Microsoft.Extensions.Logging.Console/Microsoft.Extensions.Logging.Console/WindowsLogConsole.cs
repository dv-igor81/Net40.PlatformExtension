using System;
using System.IO;

namespace Microsoft.Extensions.Logging.Console;

internal class WindowsLogConsole : IConsole
{
	private readonly TextWriter _textWriter;

	public WindowsLogConsole(bool stdErr = false)
	{
		_textWriter = (stdErr ? System.Console.Error : System.Console.Out);
	}

	private bool SetColor(ConsoleColor? background, ConsoleColor? foreground)
	{
		if (background.HasValue)
		{
			System.Console.BackgroundColor = background.Value;
		}
		if (foreground.HasValue)
		{
			System.Console.ForegroundColor = foreground.Value;
		}
		return background.HasValue || foreground.HasValue;
	}

	private void ResetColor()
	{
		System.Console.ResetColor();
	}

	public void Write(string message, ConsoleColor? background, ConsoleColor? foreground)
	{
		bool colorChanged = SetColor(background, foreground);
		_textWriter.Write(message);
		if (colorChanged)
		{
			ResetColor();
		}
	}

	public void WriteLine(string message, ConsoleColor? background, ConsoleColor? foreground)
	{
		bool colorChanged = SetColor(background, foreground);
		_textWriter.WriteLine(message);
		if (colorChanged)
		{
			ResetColor();
		}
	}

	public void Flush()
	{
	}
}
