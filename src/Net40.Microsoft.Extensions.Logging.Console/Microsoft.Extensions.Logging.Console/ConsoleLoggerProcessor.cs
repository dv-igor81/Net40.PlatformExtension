using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Microsoft.Extensions.Logging.Console;

internal class ConsoleLoggerProcessor : IDisposable
{
	private const int _maxQueuedMessages = 1024;

	private readonly BlockingCollection<LogMessageEntry> _messageQueue = new BlockingCollection<LogMessageEntry>(1024);

	private readonly Thread _outputThread;

	public IConsole Console;

	public IConsole ErrorConsole;

	public ConsoleLoggerProcessor()
	{
		_outputThread = new Thread(ProcessLogQueue)
		{
			IsBackground = true,
			Name = "Console logger queue processing thread"
		};
		_outputThread.Start();
	}

	public virtual void EnqueueMessage(LogMessageEntry message)
	{
		if (!_messageQueue.IsAddingCompleted)
		{
			try
			{
				_messageQueue.Add(message);
				return;
			}
			catch (InvalidOperationException)
			{
			}
		}
		try
		{
			WriteMessage(message);
		}
		catch (Exception)
		{
		}
	}

	internal virtual void WriteMessage(LogMessageEntry message)
	{
		IConsole console = (message.LogAsError ? ErrorConsole : Console);
		if (message.TimeStamp != null)
		{
			console.Write(message.TimeStamp, message.MessageColor, message.MessageColor);
		}
		if (message.LevelString != null)
		{
			console.Write(message.LevelString, message.LevelBackground, message.LevelForeground);
		}
		console.Write(message.Message, message.MessageColor, message.MessageColor);
		console.Flush();
	}

	private void ProcessLogQueue()
	{
		try
		{
			foreach (LogMessageEntry message in _messageQueue.GetConsumingEnumerable())
			{
				WriteMessage(message);
			}
		}
		catch
		{
			try
			{
				_messageQueue.CompleteAdding();
			}
			catch
			{
			}
		}
	}

	public void Dispose()
	{
		_messageQueue.CompleteAdding();
		try
		{
			_outputThread.Join(1500);
		}
		catch (ThreadStateException)
		{
		}
	}
}
