using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting.Internal;

namespace Microsoft.Extensions.Hosting;

public class HostBuilder : IHostBuilder
{
	private List<Action<IConfigurationBuilder>> _configureHostConfigActions = new List<Action<IConfigurationBuilder>>();

	private List<Action<HostBuilderContext, IConfigurationBuilder>> _configureAppConfigActions = new List<Action<HostBuilderContext, IConfigurationBuilder>>();

	private List<Action<HostBuilderContext, IServiceCollection>> _configureServicesActions = new List<Action<HostBuilderContext, IServiceCollection>>();

	private List<IConfigureContainerAdapter> _configureContainerActions = new List<IConfigureContainerAdapter>();

	private IServiceFactoryAdapter _serviceProviderFactory = new ServiceFactoryAdapter<IServiceCollection>(new DefaultServiceProviderFactory());

	private bool _hostBuilt;

	private IConfiguration _hostConfiguration;

	private IConfiguration _appConfiguration;

	private HostBuilderContext _hostBuilderContext;

	private HostingEnvironment _hostingEnvironment;

	private IServiceProvider _appServices;

	public IDictionary<object, object> Properties { get; } = new Dictionary<object, object>();


	public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
	{
		_configureHostConfigActions.Add(configureDelegate ?? throw new ArgumentNullException("configureDelegate"));
		return this;
	}

	public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
	{
		_configureAppConfigActions.Add(configureDelegate ?? throw new ArgumentNullException("configureDelegate"));
		return this;
	}

	public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
	{
		_configureServicesActions.Add(configureDelegate ?? throw new ArgumentNullException("configureDelegate"));
		return this;
	}

	public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory)
	{
		_serviceProviderFactory = new ServiceFactoryAdapter<TContainerBuilder>(factory ?? throw new ArgumentNullException("factory"));
		return this;
	}

	public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory)
	{
		_serviceProviderFactory = new ServiceFactoryAdapter<TContainerBuilder>(() => _hostBuilderContext, factory ?? throw new ArgumentNullException("factory"));
		return this;
	}

	public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
	{
		_configureContainerActions.Add(new ConfigureContainerAdapter<TContainerBuilder>(configureDelegate ?? throw new ArgumentNullException("configureDelegate")));
		return this;
	}

	public IHost Build()
	{
		if (_hostBuilt)
		{
			throw new InvalidOperationException("Build can only be called once.");
		}
		_hostBuilt = true;
		BuildHostConfiguration();
		CreateHostingEnvironment();
		CreateHostBuilderContext();
		BuildAppConfiguration();
		CreateServiceProvider();
		return _appServices.GetRequiredService<IHost>();
	}

	private void BuildHostConfiguration()
	{
		IConfigurationBuilder configBuilder = new ConfigurationBuilder().AddInMemoryCollection();
		foreach (Action<IConfigurationBuilder> buildAction in _configureHostConfigActions)
		{
			buildAction(configBuilder);
		}
		_hostConfiguration = configBuilder.Build();
	}

	private void CreateHostingEnvironment()
	{
		_hostingEnvironment = new HostingEnvironment
		{
			ApplicationName = _hostConfiguration[HostDefaults.ApplicationKey],
			EnvironmentName = (_hostConfiguration[HostDefaults.EnvironmentKey] ?? Environments.Production),
			ContentRootPath = ResolveContentRootPath(_hostConfiguration[HostDefaults.ContentRootKey], AppContext.BaseDirectory)
		};
		if (string.IsNullOrEmpty(_hostingEnvironment.ApplicationName))
		{
			_hostingEnvironment.ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name;
		}
		_hostingEnvironment.ContentRootFileProvider = new PhysicalFileProvider(_hostingEnvironment.ContentRootPath);
	}

	private string ResolveContentRootPath(string contentRootPath, string basePath)
	{
		if (string.IsNullOrEmpty(contentRootPath))
		{
			return basePath;
		}
		if (Path.IsPathRooted(contentRootPath))
		{
			return contentRootPath;
		}
		return Path.Combine(Path.GetFullPath(basePath), contentRootPath);
	}

	private void CreateHostBuilderContext()
	{
		_hostBuilderContext = new HostBuilderContext(Properties)
		{
			HostingEnvironment = _hostingEnvironment,
			Configuration = _hostConfiguration
		};
	}

	private void BuildAppConfiguration()
	{
		IConfigurationBuilder configBuilder = new ConfigurationBuilder().SetBasePath(_hostingEnvironment.ContentRootPath).AddConfiguration(_hostConfiguration, shouldDisposeConfiguration: true);
		foreach (Action<HostBuilderContext, IConfigurationBuilder> buildAction in _configureAppConfigActions)
		{
			buildAction(_hostBuilderContext, configBuilder);
		}
		_appConfiguration = configBuilder.Build();
		_hostBuilderContext.Configuration = _appConfiguration;
	}

	private void CreateServiceProvider()
	{
		ServiceCollection services = new ServiceCollection();
		((IServiceCollection)services).AddSingleton((IHostingEnvironment)_hostingEnvironment);
		((IServiceCollection)services).AddSingleton((IHostEnvironment)_hostingEnvironment);
		services.AddSingleton(_hostBuilderContext);
		services.AddSingleton((IServiceProvider _) => _appConfiguration);
		services.AddSingleton((IServiceProvider s) => (IApplicationLifetime)s.GetService<IHostApplicationLifetime>());
		services.AddSingleton<IHostApplicationLifetime, ApplicationLifetime>();
		services.AddSingleton<IHostLifetime, ConsoleLifetime>();
		services.AddSingleton<IHost, Microsoft.Extensions.Hosting.Internal.Host>();
		services.AddOptions();
		services.AddLogging();
		foreach (Action<HostBuilderContext, IServiceCollection> configureServicesAction in _configureServicesActions)
		{
			configureServicesAction(_hostBuilderContext, services);
		}
		object containerBuilder = _serviceProviderFactory.CreateBuilder(services);
		foreach (IConfigureContainerAdapter containerAction in _configureContainerActions)
		{
			containerAction.ConfigureContainer(_hostBuilderContext, containerBuilder);
		}
		_appServices = _serviceProviderFactory.CreateServiceProvider(containerBuilder);
		if (_appServices == null)
		{
			throw new InvalidOperationException("The IServiceProviderFactory returned a null IServiceProvider.");
		}
		_appServices.GetService<IConfiguration>();
	}
}
