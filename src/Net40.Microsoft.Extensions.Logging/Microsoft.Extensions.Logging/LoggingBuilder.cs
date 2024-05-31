using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Logging;

internal class LoggingBuilder : ILoggingBuilder
{
	public IServiceCollection Services { get; }

	public LoggingBuilder(IServiceCollection services)
	{
		Services = services;
	}
}
