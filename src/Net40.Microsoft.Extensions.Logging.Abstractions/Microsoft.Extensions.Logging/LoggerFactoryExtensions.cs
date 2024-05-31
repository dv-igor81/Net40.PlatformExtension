using System;
using Microsoft.Extensions.Internal;

namespace Microsoft.Extensions.Logging;

public static class LoggerFactoryExtensions
{
	public static ILogger<T> CreateLogger<T>(this ILoggerFactory factory)
	{
		if (factory == null)
		{
			throw new ArgumentNullException("factory");
		}
		return new Logger<T>(factory);
	}

	public static ILogger CreateLogger(this ILoggerFactory factory, Type type)
	{
		if (factory == null)
		{
			throw new ArgumentNullException("factory");
		}
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		return factory.CreateLogger(TypeNameHelper.GetTypeDisplayName(type, fullName: true, includeGenericParameterNames: false, includeGenericParameters: false, '.'));
	}
}
