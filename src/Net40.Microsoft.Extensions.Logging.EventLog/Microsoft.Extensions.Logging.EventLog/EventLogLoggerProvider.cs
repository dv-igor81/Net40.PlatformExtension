using System;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging.EventLog;

[ProviderAlias("EventLog")]
public class EventLogLoggerProvider : ILoggerProvider, IDisposable, ISupportExternalScope
{
	internal readonly EventLogSettings _settings;

	private IExternalScopeProvider _scopeProvider;

	public EventLogLoggerProvider()
		: this((EventLogSettings)null)
	{
	}

	public EventLogLoggerProvider(EventLogSettings settings)
	{
		_settings = settings ?? new EventLogSettings();
	}

	public EventLogLoggerProvider(IOptions<EventLogSettings> options)
		: this(options.Value)
	{
	}

	public ILogger CreateLogger(string name)
	{
		return new EventLogLogger(name, _settings, _scopeProvider);
	}

	public void Dispose()
	{
		if (_settings.EventLog is WindowsEventLog windowsEventLog)
		{
			windowsEventLog.DiagnosticsEventLog.Dispose();
		}
	}

	public void SetScopeProvider(IExternalScopeProvider scopeProvider)
	{
		_scopeProvider = scopeProvider;
	}
}
