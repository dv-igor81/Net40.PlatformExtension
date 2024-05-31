using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Debug;

namespace Microsoft.Extensions.Logging;

public static class DebugLoggerFactoryExtensions
{
	public static ILoggingBuilder AddDebug(this ILoggingBuilder builder)
	{
		builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, DebugLoggerProvider>());
		return builder;
	}
}
