using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Hosting;

public interface IHost : IDisposable
{
	IServiceProvider Services { get; }

	Task StartAsync(CancellationToken cancellationToken = default(CancellationToken));

	Task StopAsync(CancellationToken cancellationToken = default(CancellationToken));
}
