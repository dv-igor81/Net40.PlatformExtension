using System.Threading;

namespace Microsoft.Extensions.Hosting;

public interface IHostApplicationLifetime
{
	CancellationToken ApplicationStarted { get; }

	CancellationToken ApplicationStopping { get; }

	CancellationToken ApplicationStopped { get; }

	void StopApplication();
}
