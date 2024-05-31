using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.Logging.Configuration;

internal class LoggerProviderConfiguration<T> : ILoggerProviderConfiguration<T>
{
	public IConfiguration Configuration { get; }

	public LoggerProviderConfiguration(ILoggerProviderConfigurationFactory providerConfigurationFactory)
	{
		Configuration = providerConfigurationFactory.GetConfiguration(typeof(T));
	}
}
