using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class OptionsServiceCollectionExtensions
{
	public static IServiceCollection AddOptions(this IServiceCollection services)
	{
		if (services == null)
		{
			throw new ArgumentNullException("services");
		}
		services.TryAdd(ServiceDescriptor.Singleton(typeof(IOptions<>), typeof(OptionsManager<>)));
		services.TryAdd(ServiceDescriptor.Scoped(typeof(IOptionsSnapshot<>), typeof(OptionsManager<>)));
		services.TryAdd(ServiceDescriptor.Singleton(typeof(IOptionsMonitor<>), typeof(OptionsMonitor<>)));
		services.TryAdd(ServiceDescriptor.Transient(typeof(IOptionsFactory<>), typeof(OptionsFactory<>)));
		services.TryAdd(ServiceDescriptor.Singleton(typeof(IOptionsMonitorCache<>), typeof(OptionsCache<>)));
		return services;
	}

	public static IServiceCollection Configure<TOptions>(this IServiceCollection services, Action<TOptions> configureOptions) where TOptions : class
	{
		return services.Configure(Microsoft.Extensions.Options.Options.DefaultName, configureOptions);
	}

	public static IServiceCollection Configure<TOptions>(this IServiceCollection services, string name, Action<TOptions> configureOptions) where TOptions : class
	{
		if (services == null)
		{
			throw new ArgumentNullException("services");
		}
		if (configureOptions == null)
		{
			throw new ArgumentNullException("configureOptions");
		}
		services.AddOptions();
		services.AddSingleton((IConfigureOptions<TOptions>)new ConfigureNamedOptions<TOptions>(name, configureOptions));
		return services;
	}

	public static IServiceCollection ConfigureAll<TOptions>(this IServiceCollection services, Action<TOptions> configureOptions) where TOptions : class
	{
		return services.Configure(null, configureOptions);
	}

	public static IServiceCollection PostConfigure<TOptions>(this IServiceCollection services, Action<TOptions> configureOptions) where TOptions : class
	{
		return services.PostConfigure(Microsoft.Extensions.Options.Options.DefaultName, configureOptions);
	}

	public static IServiceCollection PostConfigure<TOptions>(this IServiceCollection services, string name, Action<TOptions> configureOptions) where TOptions : class
	{
		if (services == null)
		{
			throw new ArgumentNullException("services");
		}
		if (configureOptions == null)
		{
			throw new ArgumentNullException("configureOptions");
		}
		services.AddOptions();
		services.AddSingleton((IPostConfigureOptions<TOptions>)new PostConfigureOptions<TOptions>(name, configureOptions));
		return services;
	}

	public static IServiceCollection PostConfigureAll<TOptions>(this IServiceCollection services, Action<TOptions> configureOptions) where TOptions : class
	{
		return services.PostConfigure(null, configureOptions);
	}

	public static IServiceCollection ConfigureOptions<TConfigureOptions>(this IServiceCollection services) where TConfigureOptions : class
	{
		return services.ConfigureOptions(typeof(TConfigureOptions));
	}

	private static bool IsAction(Type type)
	{
		return IntrospectionExtensions.GetTypeInfo(type).IsGenericType && type.GetGenericTypeDefinition() == typeof(Action<>);
	}

	private static IEnumerable<Type> FindIConfigureOptions(Type type)
	{
		IEnumerable<Type> serviceTypes = IntrospectionExtensions.GetTypeInfo(type).ImplementedInterfaces.Where((Type t) => IntrospectionExtensions.GetTypeInfo(t).IsGenericType && (t.GetGenericTypeDefinition() == typeof(IConfigureOptions<>) || t.GetGenericTypeDefinition() == typeof(IPostConfigureOptions<>)));
		if (!serviceTypes.Any())
		{
			throw new InvalidOperationException(IsAction(type) ? "Resources.Error_NoIConfigureOptionsAndAction" : "Resources.Error_NoIConfigureOptions");
		}
		return serviceTypes;
	}

	public static IServiceCollection ConfigureOptions(this IServiceCollection services, Type configureType)
	{
		services.AddOptions();
		IEnumerable<Type> serviceTypes = FindIConfigureOptions(configureType);
		foreach (Type serviceType in serviceTypes)
		{
			services.AddTransient(serviceType, configureType);
		}
		return services;
	}

	public static IServiceCollection ConfigureOptions(this IServiceCollection services, object configureInstance)
	{
		services.AddOptions();
		IEnumerable<Type> serviceTypes = FindIConfigureOptions(configureInstance.GetType());
		foreach (Type serviceType in serviceTypes)
		{
			services.AddSingleton(serviceType, configureInstance);
		}
		return services;
	}

	public static OptionsBuilder<TOptions> AddOptions<TOptions>(this IServiceCollection services) where TOptions : class
	{
		return services.AddOptions<TOptions>(Microsoft.Extensions.Options.Options.DefaultName);
	}

	public static OptionsBuilder<TOptions> AddOptions<TOptions>(this IServiceCollection services, string name) where TOptions : class
	{
		if (services == null)
		{
			throw new ArgumentNullException("services");
		}
		services.AddOptions();
		return new OptionsBuilder<TOptions>(services, name);
	}
}
