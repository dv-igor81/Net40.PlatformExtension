using System;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Hosting.Internal;

public class ApplicationLifetime : IApplicationLifetime, IHostApplicationLifetime
{
	private readonly CancellationTokenSource _startedSource = new CancellationTokenSource();

	private readonly CancellationTokenSource _stoppingSource = new CancellationTokenSource();

	private readonly CancellationTokenSource _stoppedSource = new CancellationTokenSource();

	private readonly ILogger<ApplicationLifetime> _logger;

	public CancellationToken ApplicationStarted => _startedSource.Token;

	public CancellationToken ApplicationStopping => _stoppingSource.Token;

	public CancellationToken ApplicationStopped => _stoppedSource.Token;

	public ApplicationLifetime(ILogger<ApplicationLifetime> logger)
	{
		_logger = logger;
	}

	public void StopApplication()
	{
		lock (_stoppingSource)
		{
			try
			{
				ExecuteHandlers(_stoppingSource);
			}
			catch (Exception ex)
			{
				_logger.ApplicationError(7, "An error occurred stopping the application", ex);
			}
		}
	}

	public void NotifyStarted()
	{
		try
		{
			ExecuteHandlers(_startedSource);
		}
		catch (Exception ex)
		{
			_logger.ApplicationError(6, "An error occurred starting the application", ex);
		}
	}

	public void NotifyStopped()
	{
		try
		{
			ExecuteHandlers(_stoppedSource);
		}
		catch (Exception ex)
		{
			_logger.ApplicationError(8, "An error occurred stopping the application", ex);
		}
	}

	private void ExecuteHandlers(CancellationTokenSource cancel)
	{
		if (!cancel.IsCancellationRequested)
		{
			cancel.Cancel(throwOnFirstException: false);
		}
	}
}
