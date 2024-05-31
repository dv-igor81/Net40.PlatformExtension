using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging.Configuration;

internal class LoggerProviderConfigureOptions<TOptions, TProvider> : ConfigureFromConfigurationOptions<TOptions> where TOptions : class
{
	public LoggerProviderConfigureOptions(ILoggerProviderConfiguration<TProvider> providerConfiguration)
		: base(providerConfiguration.Configuration)
	{
	}
}
