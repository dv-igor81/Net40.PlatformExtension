using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging;

public class LoggerFactory : ILoggerFactory, IDisposable
{
	private struct ProviderRegistration
	{
		public ILoggerProvider Provider;

		public bool ShouldDispose;
	}

	private class DisposingLoggerFactory : ILoggerFactory, IDisposable
	{
		private readonly ILoggerFactory _loggerFactory;

		private readonly ServiceProvider _serviceProvider;

		public DisposingLoggerFactory(ILoggerFactory loggerFactory, ServiceProvider serviceProvider)
		{
			_loggerFactory = loggerFactory;
			_serviceProvider = serviceProvider;
		}

		public void Dispose()
		{
			_serviceProvider.Dispose();
		}

		public ILogger CreateLogger(string categoryName)
		{
			return _loggerFactory.CreateLogger(categoryName);
		}

		public void AddProvider(ILoggerProvider provider)
		{
			_loggerFactory.AddProvider(provider);
		}
	}

	private static readonly LoggerRuleSelector RuleSelector = new LoggerRuleSelector();

	private readonly Dictionary<string, Logger> _loggers = new Dictionary<string, Logger>(StringComparer.Ordinal);

	private readonly List<ProviderRegistration> _providerRegistrations = new List<ProviderRegistration>();

	private readonly object _sync = new object();

	private volatile bool _disposed;

	private IDisposable _changeTokenRegistration;

	private LoggerFilterOptions _filterOptions;

	private LoggerExternalScopeProvider _scopeProvider;

	public LoggerFactory()
		: this(Enumerable.Empty<ILoggerProvider>())
	{
	}

	public LoggerFactory(IEnumerable<ILoggerProvider> providers)
		: this(providers, new StaticFilterOptionsMonitor(new LoggerFilterOptions()))
	{
	}

	public LoggerFactory(IEnumerable<ILoggerProvider> providers, LoggerFilterOptions filterOptions)
		: this(providers, new StaticFilterOptionsMonitor(filterOptions))
	{
	}

	public LoggerFactory(IEnumerable<ILoggerProvider> providers, IOptionsMonitor<LoggerFilterOptions> filterOption)
	{
		foreach (ILoggerProvider provider in providers)
		{
			AddProviderRegistration(provider, dispose: false);
		}
		_changeTokenRegistration = filterOption.OnChange(RefreshFilters);
		RefreshFilters(filterOption.CurrentValue);
	}

	public static ILoggerFactory Create(Action<ILoggingBuilder> configure)
	{
		ServiceCollection serviceCollection = new ServiceCollection();
		serviceCollection.AddLogging(configure);
		ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
		ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();
		return new DisposingLoggerFactory(loggerFactory, serviceProvider);
	}

	private void RefreshFilters(LoggerFilterOptions filterOptions)
	{
		lock (_sync)
		{
			_filterOptions = filterOptions;
			foreach (KeyValuePair<string, Logger> logger3 in _loggers)
			{
				Logger logger = logger3.Value;
				(logger.MessageLoggers, logger.ScopeLoggers) = ApplyFilters(logger.Loggers);
			}
		}
	}

	public ILogger CreateLogger(string categoryName)
	{
		if (CheckDisposed())
		{
			throw new ObjectDisposedException("LoggerFactory");
		}
		lock (_sync)
		{
			if (!_loggers.TryGetValue(categoryName, out var logger))
			{
				logger = new Logger
				{
					Loggers = CreateLoggers(categoryName)
				};
				(logger.MessageLoggers, logger.ScopeLoggers) = ApplyFilters(logger.Loggers);
				_loggers[categoryName] = logger;
			}
			return logger;
		}
	}

	public void AddProvider(ILoggerProvider provider)
	{
		if (CheckDisposed())
		{
			throw new ObjectDisposedException("LoggerFactory");
		}
		lock (_sync)
		{
			AddProviderRegistration(provider, dispose: true);
			foreach (KeyValuePair<string, Logger> existingLogger in _loggers)
			{
				Logger logger = existingLogger.Value;
				LoggerInformation[] loggerInformation = logger.Loggers;
				int newLoggerIndex = loggerInformation.Length;
				Array.Resize(ref loggerInformation, loggerInformation.Length + 1);
				loggerInformation[newLoggerIndex] = new LoggerInformation(provider, existingLogger.Key);
				logger.Loggers = loggerInformation;
				(logger.MessageLoggers, logger.ScopeLoggers) = ApplyFilters(logger.Loggers);
			}
		}
	}

	private void AddProviderRegistration(ILoggerProvider provider, bool dispose)
	{
		_providerRegistrations.Add(new ProviderRegistration
		{
			Provider = provider,
			ShouldDispose = dispose
		});
		if (provider is ISupportExternalScope supportsExternalScope)
		{
			if (_scopeProvider == null)
			{
				_scopeProvider = new LoggerExternalScopeProvider();
			}
			supportsExternalScope.SetScopeProvider(_scopeProvider);
		}
	}

	private LoggerInformation[] CreateLoggers(string categoryName)
	{
		LoggerInformation[] loggers = new LoggerInformation[_providerRegistrations.Count];
		for (int i = 0; i < _providerRegistrations.Count; i++)
		{
			loggers[i] = new LoggerInformation(_providerRegistrations[i].Provider, categoryName);
		}
		return loggers;
	}

	private (MessageLogger[] MessageLoggers, ScopeLogger[] ScopeLoggers) ApplyFilters(LoggerInformation[] loggers)
	{
		List<MessageLogger> messageLoggers = new List<MessageLogger>();
		List<ScopeLogger> scopeLoggers = (_filterOptions.CaptureScopes ? new List<ScopeLogger>() : null);
		for (int i = 0; i < loggers.Length; i++)
		{
			LoggerInformation loggerInformation = loggers[i];
			RuleSelector.Select(_filterOptions, loggerInformation.ProviderType, loggerInformation.Category, out var minLevel, out var filter);
			if (!minLevel.HasValue || !(minLevel > LogLevel.Critical))
			{
				messageLoggers.Add(new MessageLogger(loggerInformation.Logger, loggerInformation.Category, loggerInformation.ProviderType.FullName, minLevel, filter));
				if (!loggerInformation.ExternalScope)
				{
					scopeLoggers?.Add(new ScopeLogger(loggerInformation.Logger, null));
				}
			}
		}
		if (_scopeProvider != null)
		{
			scopeLoggers?.Add(new ScopeLogger(null, _scopeProvider));
		}
		return (MessageLoggers: messageLoggers.ToArray(), ScopeLoggers: scopeLoggers?.ToArray());
	}

	protected virtual bool CheckDisposed()
	{
		return _disposed;
	}

	public void Dispose()
	{
		if (_disposed)
		{
			return;
		}
		_disposed = true;
		_changeTokenRegistration?.Dispose();
		foreach (ProviderRegistration registration in _providerRegistrations)
		{
			try
			{
				if (registration.ShouldDispose)
				{
					registration.Provider.Dispose();
				}
			}
			catch
			{
			}
		}
	}
}
