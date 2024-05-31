using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Hosting;

public abstract class BackgroundService : IHostedService, IDisposable
{
	private Task _executingTask;

	private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

	protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

	public virtual Task StartAsync(CancellationToken cancellationToken)
	{
		_executingTask = ExecuteAsync(_stoppingCts.Token);
		if (_executingTask.IsCompleted)
		{
			return _executingTask;
		}
		return TaskExEx.CompletedTask;
	}

	public virtual async Task StopAsync(CancellationToken cancellationToken)
	{
		if (_executingTask != null)
		{
			try
			{
				_stoppingCts.Cancel();
			}
			finally
			{
				await TaskEx.WhenAny(new Task[2]
				{
					_executingTask,
					TaskEx.Delay(-1, cancellationToken)
				});
			}
		}
	}

	public virtual void Dispose()
	{
		_stoppingCts.Cancel();
	}
}
