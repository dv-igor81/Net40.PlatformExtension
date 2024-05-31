using System;
using System.IO;

namespace Microsoft.Extensions.Logging.Console;

internal class AnsiSystemConsole : IAnsiSystemConsole
{
	private readonly TextWriter _textWriter;

	public AnsiSystemConsole(bool stdErr = false)
	{
		_textWriter = (stdErr ? System.Console.Error : System.Console.Out);
	}

	public void Write(string message)
	{
		_textWriter.Write(message);
	}
}
