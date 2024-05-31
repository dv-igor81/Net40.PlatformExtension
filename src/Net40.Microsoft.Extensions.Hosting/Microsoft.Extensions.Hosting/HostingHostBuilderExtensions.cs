using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Hosting;

public static class HostingHostBuilderExtensions
{
	public static IHostBuilder UseEnvironment(this IHostBuilder hostBuilder, string environment)
	{
		return hostBuilder.ConfigureHostConfiguration(delegate(IConfigurationBuilder configBuilder)
		{
			configBuilder.AddInMemoryCollection(new KeyValuePair<string, string>[1]
			{
				new KeyValuePair<string, string>(HostDefaults.EnvironmentKey, environment ?? throw new ArgumentNullException("environment"))
			});
		});
	}

	public static IHostBuilder UseContentRoot(this IHostBuilder hostBuilder, string contentRoot)
	{
		return hostBuilder.ConfigureHostConfiguration(delegate(IConfigurationBuilder configBuilder)
		{
			configBuilder.AddInMemoryCollection(new KeyValuePair<string, string>[1]
			{
				new KeyValuePair<string, string>(HostDefaults.ContentRootKey, contentRoot ?? throw new ArgumentNullException("contentRoot"))
			});
		});
	}

	public static IHostBuilder UseDefaultServiceProvider(this IHostBuilder hostBuilder, Action<ServiceProviderOptions> configure)
	{
		return hostBuilder.UseDefaultServiceProvider(delegate(HostBuilderContext context, ServiceProviderOptions options)
		{
			configure(options);
		});
	}

	public static IHostBuilder UseDefaultServiceProvider(this IHostBuilder hostBuilder, Action<HostBuilderContext, ServiceProviderOptions> configure)
	{
		return hostBuilder.UseServiceProviderFactory(delegate(HostBuilderContext context)
		{
			ServiceProviderOptions serviceProviderOptions = new ServiceProviderOptions();
			configure(context, serviceProviderOptions);
			return new DefaultServiceProviderFactory(serviceProviderOptions);
		});
	}

	public static IHostBuilder ConfigureLogging(this IHostBuilder hostBuilder, Action<HostBuilderContext, ILoggingBuilder> configureLogging)
	{
		return hostBuilder.ConfigureServices(delegate(HostBuilderContext context, IServiceCollection collection)
		{
			collection.AddLogging(delegate(ILoggingBuilder builder)
			{
				configureLogging(context, builder);
			});
		});
	}

	public static IHostBuilder ConfigureLogging(this IHostBuilder hostBuilder, Action<ILoggingBuilder> configureLogging)
	{
		return hostBuilder.ConfigureServices(delegate(HostBuilderContext context, IServiceCollection collection)
		{
			collection.AddLogging(delegate(ILoggingBuilder builder)
			{
				configureLogging(builder);
			});
		});
	}

	public static IHostBuilder ConfigureAppConfiguration(this IHostBuilder hostBuilder, Action<IConfigurationBuilder> configureDelegate)
	{
		return hostBuilder.ConfigureAppConfiguration(delegate(HostBuilderContext context, IConfigurationBuilder builder)
		{
			configureDelegate(builder);
		});
	}

	public static IHostBuilder ConfigureServices(this IHostBuilder hostBuilder, Action<IServiceCollection> configureDelegate)
	{
		return hostBuilder.ConfigureServices(delegate(HostBuilderContext context, IServiceCollection collection)
		{
			configureDelegate(collection);
		});
	}

	public static IHostBuilder ConfigureContainer<TContainerBuilder>(this IHostBuilder hostBuilder, Action<TContainerBuilder> configureDelegate)
	{
		return hostBuilder.ConfigureContainer(delegate(HostBuilderContext context, TContainerBuilder builder)
		{
			configureDelegate(builder);
		});
	}

	public static IHostBuilder UseConsoleLifetime(this IHostBuilder hostBuilder)
	{
		return hostBuilder.ConfigureServices(delegate(HostBuilderContext context, IServiceCollection collection)
		{
			collection.AddSingleton<IHostLifetime, ConsoleLifetime>();
		});
	}

	public static IHostBuilder UseConsoleLifetime(this IHostBuilder hostBuilder, Action<ConsoleLifetimeOptions> configureOptions)
	{
		return hostBuilder.ConfigureServices(delegate(HostBuilderContext context, IServiceCollection collection)
		{
			collection.AddSingleton<IHostLifetime, ConsoleLifetime>();
			collection.Configure(configureOptions);
		});
	}

	public static Task RunConsoleAsync(this IHostBuilder hostBuilder, CancellationToken cancellationToken = default(CancellationToken))
	{
		return hostBuilder.UseConsoleLifetime().Build().RunAsync(cancellationToken);
	}

	public static Task RunConsoleAsync(this IHostBuilder hostBuilder, Action<ConsoleLifetimeOptions> configureOptions, CancellationToken cancellationToken = default(CancellationToken))
	{
		return hostBuilder.UseConsoleLifetime(configureOptions).Build().RunAsync(cancellationToken);
	}
}
