using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Internal;
using Net40.Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection.ServiceLookup;

internal class ServiceProviderEngineScope : IServiceScope, IDisposable, IServiceProvider, IAsyncDisposable
{
	internal Action<object> _captureDisposableCallback;

	private List<object> _disposables;

	private bool _disposed;

	internal Dictionary<ServiceCacheKey, object> ResolvedServices { get; } = new Dictionary<ServiceCacheKey, object>();


	public ServiceProviderEngine Engine { get; }

	public IServiceProvider ServiceProvider => this;

	public ServiceProviderEngineScope(ServiceProviderEngine engine)
	{
		Engine = engine;
	}

	public object GetService(Type serviceType)
	{
		if (_disposed)
		{
			ThrowHelper.ThrowObjectDisposedException();
		}
		return Engine.GetService(serviceType, this);
	}

	internal object CaptureDisposable(object service)
	{
		Debug.Assert(!_disposed);
		_captureDisposableCallback?.Invoke(service);
		if (this == service || (!(service is IDisposable) && !(service is IAsyncDisposable)))
		{
			return service;
		}
		lock (ResolvedServices)
		{
			if (_disposables == null)
			{
				_disposables = new List<object>();
			}
			_disposables.Add(service);
		}
		return service;
	}

	public void Dispose()
	{
		List<object> toDispose = BeginDispose();
		if (toDispose == null)
		{
			return;
		}
		int i = toDispose.Count - 1;
		while (i >= 0)
		{
			if (toDispose[i] is IDisposable disposable)
			{
				disposable.Dispose();
				i--;
				continue;
			}
			throw new InvalidOperationException(Resources.FormatAsyncDisposableServiceDispose(TypeNameHelper.GetTypeDisplayName(toDispose[i])));
		}
	}

	public ValueTask DisposeAsync()
	{
		List<object> toDispose = BeginDispose();
		if (toDispose != null)
		{
			try
			{
				for (int j = toDispose.Count - 1; j >= 0; j--)
				{
					object disposable = toDispose[j];
					if (disposable is IAsyncDisposable asyncDisposable)
					{
						ValueTask vt2 = asyncDisposable.DisposeAsync();
						if (!vt2.IsCompletedSuccessfully)
						{
							return Await(j, vt2);
						}
						vt2.GetAwaiter().GetResult();
					}
					else
					{
						((IDisposable)disposable).Dispose();
					}
				}
			}
			catch (Exception ex)
			{
				return new ValueTask(TaskExEx.FromException(ex));
			}
		}
		return default(ValueTask);
		async ValueTask Await(int i, ValueTask vt)
		{
			await vt;
			while (i >= 0)
			{
				object disposable2 = toDispose[i];
				if (disposable2 is IAsyncDisposable asyncDisposable2)
				{
					await asyncDisposable2.DisposeAsync();
				}
				else
				{
					((IDisposable)disposable2).Dispose();
				}
				i--;
			}
		}
	}

	private List<object> BeginDispose()
	{
		List<object> toDispose;
		lock (ResolvedServices)
		{
			if (_disposed)
			{
				return null;
			}
			_disposed = true;
			toDispose = _disposables;
			_disposables = null;
		}
		return toDispose;
	}
}
