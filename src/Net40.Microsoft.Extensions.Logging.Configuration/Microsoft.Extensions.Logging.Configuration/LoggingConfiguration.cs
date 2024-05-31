using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.Logging.Configuration;

internal class LoggingConfiguration
{
	public IConfiguration Configuration { get; }

	public LoggingConfiguration(IConfiguration configuration)
	{
		Configuration = configuration;
	}
}
