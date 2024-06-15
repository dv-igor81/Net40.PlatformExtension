using System.Threading;

namespace Microsoft.Extensions.Hosting;

//[Obsolete("This type is obsolete and will be removed in a future version. The recommended alternative is Microsoft.Extensions.Hosting.IHostApplicationLifetime.", false)]
public interface IApplicationLifetime
{
	CancellationToken ApplicationStarted { get; }

	CancellationToken ApplicationStopping { get; }

	CancellationToken ApplicationStopped { get; }

	void StopApplication();
}
