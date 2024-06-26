using System;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting.Internal;

internal class ServiceFactoryAdapter<TContainerBuilder> : IServiceFactoryAdapter
{
	private IServiceProviderFactory<TContainerBuilder> _serviceProviderFactory;

	private readonly Func<HostBuilderContext> _contextResolver;

	private Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> _factoryResolver;

	public ServiceFactoryAdapter(IServiceProviderFactory<TContainerBuilder> serviceProviderFactory)
	{
		_serviceProviderFactory = serviceProviderFactory ?? throw new ArgumentNullException("serviceProviderFactory");
	}

	public ServiceFactoryAdapter(Func<HostBuilderContext> contextResolver, Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factoryResolver)
	{
		_contextResolver = contextResolver ?? throw new ArgumentNullException("contextResolver");
		_factoryResolver = factoryResolver ?? throw new ArgumentNullException("factoryResolver");
	}

	public object CreateBuilder(IServiceCollection services)
	{
		if (_serviceProviderFactory == null)
		{
			_serviceProviderFactory = _factoryResolver(_contextResolver());
			if (_serviceProviderFactory == null)
			{
				throw new InvalidOperationException("The resolver returned a null IServiceProviderFactory");
			}
		}
		return _serviceProviderFactory.CreateBuilder(services);
	}

	public IServiceProvider CreateServiceProvider(object containerBuilder)
	{
		if (_serviceProviderFactory == null)
		{
			throw new InvalidOperationException("CreateBuilder must be called before CreateServiceProvider");
		}
		return _serviceProviderFactory.CreateServiceProvider((TContainerBuilder)containerBuilder);
	}
}
