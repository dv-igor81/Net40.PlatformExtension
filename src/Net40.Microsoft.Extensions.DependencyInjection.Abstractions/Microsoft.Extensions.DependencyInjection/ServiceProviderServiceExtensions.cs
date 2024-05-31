using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceProviderServiceExtensions
{
	public static T GetService<T>(this IServiceProvider provider)
	{
		if (provider == null)
		{
			throw new ArgumentNullException("provider");
		}
		return (T)provider.GetService(typeof(T));
	}

	public static object GetRequiredService(this IServiceProvider provider, Type serviceType)
	{
		if (provider == null)
		{
			throw new ArgumentNullException("provider");
		}
		if (serviceType == null)
		{
			throw new ArgumentNullException("serviceType");
		}
		if (provider is ISupportRequiredService supportRequiredService)
		{
			return supportRequiredService.GetRequiredService(serviceType);
		}
		return provider.GetService(serviceType) ?? throw new InvalidOperationException(Resources.FormatNoServiceRegistered(serviceType));
	}

	public static T GetRequiredService<T>(this IServiceProvider provider)
	{
		if (provider == null)
		{
			throw new ArgumentNullException("provider");
		}
		return (T)provider.GetRequiredService(typeof(T));
	}

	public static IEnumerable<T> GetServices<T>(this IServiceProvider provider)
	{
		if (provider == null)
		{
			throw new ArgumentNullException("provider");
		}
		return provider.GetRequiredService<IEnumerable<T>>();
	}

	public static IEnumerable<object> GetServices(this IServiceProvider provider, Type serviceType)
	{
		if (provider == null)
		{
			throw new ArgumentNullException("provider");
		}
		if (serviceType == null)
		{
			throw new ArgumentNullException("serviceType");
		}
		Type serviceType2 = typeof(IEnumerable<>).MakeGenericType(serviceType);
		return (IEnumerable<object>)provider.GetRequiredService(serviceType2);
	}

	public static IServiceScope CreateScope(this IServiceProvider provider)
	{
		return provider.GetRequiredService<IServiceScopeFactory>().CreateScope();
	}
}
