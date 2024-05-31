using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Options;

public class OptionsMonitor<TOptions> : IOptionsMonitor<TOptions>, IDisposable where TOptions : class, new()
{
	internal class ChangeTrackerDisposable : IDisposable
	{
		private readonly Action<TOptions, string> _listener;

		private readonly OptionsMonitor<TOptions> _monitor;

		public ChangeTrackerDisposable(OptionsMonitor<TOptions> monitor, Action<TOptions, string> listener)
		{
			_listener = listener;
			_monitor = monitor;
		}

		public void OnChange(TOptions options, string name)
		{
			_listener(options, name);
		}

		public void Dispose()
		{
			_monitor._onChange -= OnChange;
		}
	}

	private readonly IOptionsMonitorCache<TOptions> _cache;

	private readonly IOptionsFactory<TOptions> _factory;

	private readonly IEnumerable<IOptionsChangeTokenSource<TOptions>> _sources;

	private readonly List<IDisposable> _registrations = new List<IDisposable>();

	public TOptions CurrentValue => Get(Options.DefaultName);

	internal event Action<TOptions, string> _onChange;

	public OptionsMonitor(IOptionsFactory<TOptions> factory, IEnumerable<IOptionsChangeTokenSource<TOptions>> sources, IOptionsMonitorCache<TOptions> cache)
	{
		_factory = factory;
		_sources = sources;
		_cache = cache;
		foreach (IOptionsChangeTokenSource<TOptions> source in _sources)
		{
			IDisposable registration = ChangeToken.OnChange(() => source.GetChangeToken(), delegate(string name)
			{
				InvokeChanged(name);
			}, source.Name);
			_registrations.Add(registration);
		}
	}

	private void InvokeChanged(string name)
	{
		name = name ?? Options.DefaultName;
		_cache.TryRemove(name);
		TOptions options = Get(name);
		if (this._onChange != null)
		{
			this._onChange(options, name);
		}
	}

	public virtual TOptions Get(string name)
	{
		name = name ?? Options.DefaultName;
		return _cache.GetOrAdd(name, () => _factory.Create(name));
	}

	public IDisposable OnChange(Action<TOptions, string> listener)
	{
		ChangeTrackerDisposable disposable = new ChangeTrackerDisposable(this, listener);
		_onChange += disposable.OnChange;
		return disposable;
	}

	public void Dispose()
	{
		foreach (IDisposable registration in _registrations)
		{
			registration.Dispose();
		}
		_registrations.Clear();
	}
}
