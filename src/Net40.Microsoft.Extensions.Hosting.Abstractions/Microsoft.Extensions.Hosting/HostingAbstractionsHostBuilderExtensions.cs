using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Hosting;

public static class HostingAbstractionsHostBuilderExtensions
{
	public static IHost Start(this IHostBuilder hostBuilder)
	{
		return TaskTheraotExtensions.GetAwaiter(hostBuilder.StartAsync()).GetResult();
	}

	public static async Task<IHost> StartAsync(this IHostBuilder hostBuilder, CancellationToken cancellationToken = default(CancellationToken))
	{
		IHost host = hostBuilder.Build();
		await host.StartAsync(cancellationToken);
		return host;
	}
}
