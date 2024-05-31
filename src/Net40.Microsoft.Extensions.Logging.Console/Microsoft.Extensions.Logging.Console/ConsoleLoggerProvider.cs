using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging.Console;

[ProviderAlias("Console")]
public class ConsoleLoggerProvider : ILoggerProvider, IDisposable, ISupportExternalScope
{
	private readonly IOptionsMonitor<ConsoleLoggerOptions> _options;

	private readonly ConcurrentDictionary<string, ConsoleLogger> _loggers;

	private readonly ConsoleLoggerProcessor _messageQueue;

	private IDisposable _optionsReloadToken;

	private IExternalScopeProvider _scopeProvider = NullExternalScopeProvider.Instance;

	public ConsoleLoggerProvider(IOptionsMonitor<ConsoleLoggerOptions> options)
	{
		_options = options;
		_loggers = new ConcurrentDictionary<string, ConsoleLogger>();
		ReloadLoggerOptions(options.CurrentValue);
		_optionsReloadToken = _options.OnChange(ReloadLoggerOptions);
		_messageQueue = new ConsoleLoggerProcessor();
		_messageQueue.Console = new WindowsLogConsole();
		_messageQueue.ErrorConsole = new WindowsLogConsole(stdErr: true);
	}

	private void ReloadLoggerOptions(ConsoleLoggerOptions options)
	{
		foreach (KeyValuePair<string, ConsoleLogger> logger in _loggers)
		{
			logger.Value.Options = options;
		}
	}

	public ILogger CreateLogger(string name)
	{
		return _loggers.GetOrAdd(name, (string loggerName) => new ConsoleLogger(name, _messageQueue)
		{
			Options = _options.CurrentValue,
			ScopeProvider = _scopeProvider
		});
	}

	public void Dispose()
	{
		_optionsReloadToken?.Dispose();
		_messageQueue.Dispose();
	}

	public void SetScopeProvider(IExternalScopeProvider scopeProvider)
	{
		_scopeProvider = scopeProvider;
		foreach (KeyValuePair<string, ConsoleLogger> logger in _loggers)
		{
			logger.Value.ScopeProvider = _scopeProvider;
		}
	}
}
