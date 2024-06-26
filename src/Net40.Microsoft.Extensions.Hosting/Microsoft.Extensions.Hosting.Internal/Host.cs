using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Hosting.Internal;

internal class Host : IHost, IDisposable, IAsyncDisposable
{
	private readonly ILogger<Host> _logger;

	private readonly IHostLifetime _hostLifetime;

	private readonly ApplicationLifetime _applicationLifetime;

	private readonly HostOptions _options;

	private IEnumerable<IHostedService> _hostedServices;

	public IServiceProvider Services { get; }

	public Host(IServiceProvider services, IHostApplicationLifetime applicationLifetime, ILogger<Host> logger, IHostLifetime hostLifetime, IOptions<HostOptions> options)
	{
		Services = services ?? throw new ArgumentNullException("services");
		_applicationLifetime = (applicationLifetime ?? throw new ArgumentNullException("applicationLifetime")) as ApplicationLifetime;
		_logger = logger ?? throw new ArgumentNullException("logger");
		_hostLifetime = hostLifetime ?? throw new ArgumentNullException("hostLifetime");
		_options = options?.Value ?? throw new ArgumentNullException("options");
	}

	public async Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		_logger.Starting();
		using CancellationTokenSource combinedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _applicationLifetime.ApplicationStopping);
		CancellationToken combinedCancellationToken = combinedCancellationTokenSource.Token;
		await _hostLifetime.WaitForStartAsync(combinedCancellationToken);
		combinedCancellationToken.ThrowIfCancellationRequested();
		_hostedServices = Services.GetService<IEnumerable<IHostedService>>();
		foreach (IHostedService hostedService in _hostedServices)
		{
			await TaskTheraotExtensions.ConfigureAwait(hostedService.StartAsync(combinedCancellationToken), continueOnCapturedContext: false);
		}
		_applicationLifetime?.NotifyStarted();
		_logger.Started();
	}

	public async Task StopAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		_logger.Stopping();
		using (CancellationTokenSource cts = CancellationTokenSourceTheraotExtensions.CancelAfter(new CancellationTokenSource(), _options.ShutdownTimeout))
		{
			using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken);
			CancellationToken token = linkedCts.Token;
			_applicationLifetime?.StopApplication();
			IList<Exception> exceptions = new List<Exception>();
			if (_hostedServices != null)
			{
				foreach (IHostedService hostedService in _hostedServices.Reverse())
				{
					token.ThrowIfCancellationRequested();
					try
					{
						await TaskTheraotExtensions.ConfigureAwait(hostedService.StopAsync(token), continueOnCapturedContext: false);
					}
					catch (Exception ex2)
					{
						exceptions.Add(ex2);
					}
				}
			}
			token.ThrowIfCancellationRequested();
			await _hostLifetime.StopAsync(token);
			_applicationLifetime?.NotifyStopped();
			if (exceptions.Count > 0)
			{
				AggregateException ex = new AggregateException("One or more hosted services failed to stop.", exceptions);
				_logger.StoppedWithException(ex);
				throw ex;
			}
		}
		_logger.Stopped();
	}

	public void Dispose()
	{
		DisposeAsync().GetAwaiter().GetResult();
	}

	public async ValueTask DisposeAsync()
	{
		IServiceProvider services = Services;
		IServiceProvider serviceProvider = services;
		if (!(serviceProvider is IAsyncDisposable asyncDisposable))
		{
			if (serviceProvider is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
		else
		{
			await asyncDisposable.DisposeAsync();
		}
	}
}
