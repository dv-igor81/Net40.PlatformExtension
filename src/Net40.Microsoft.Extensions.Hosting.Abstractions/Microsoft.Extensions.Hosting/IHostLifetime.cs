using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Hosting;

public interface IHostLifetime
{
	Task WaitForStartAsync(CancellationToken cancellationToken);

	Task StopAsync(CancellationToken cancellationToken);
}
