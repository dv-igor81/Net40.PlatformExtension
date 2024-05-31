using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting;

public interface IHostBuilder
{
	IDictionary<object, object> Properties { get; }

	IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate);

	IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate);

	IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate);

	IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory);

	IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory);

	IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate);

	IHost Build();
}
