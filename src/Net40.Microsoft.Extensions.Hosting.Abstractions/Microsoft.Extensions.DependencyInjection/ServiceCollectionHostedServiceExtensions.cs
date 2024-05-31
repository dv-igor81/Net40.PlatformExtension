using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionHostedServiceExtensions
{
	public static IServiceCollection AddHostedService<THostedService>(this IServiceCollection services) where THostedService : class, IHostedService
	{
		services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, THostedService>());
		return services;
	}

	public static IServiceCollection AddHostedService<THostedService>(this IServiceCollection services, Func<IServiceProvider, THostedService> implementationFactory) where THostedService : class, IHostedService
	{
		services.TryAddEnumerable(ServiceDescriptor.Singleton((Func<IServiceProvider, IHostedService>)implementationFactory));
		return services;
	}
}
