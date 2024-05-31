using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.Extensions.Configuration;

public static class UserSecretsConfigurationExtensions
{
	public static IConfigurationBuilder AddUserSecrets<T>(this IConfigurationBuilder configuration) where T : class
	{
		return configuration.AddUserSecrets(IntrospectionExtensions.GetTypeInfo(typeof(T)).Assembly, optional: false, reloadOnChange: false);
	}

	public static IConfigurationBuilder AddUserSecrets<T>(this IConfigurationBuilder configuration, bool optional) where T : class
	{
		return configuration.AddUserSecrets(IntrospectionExtensions.GetTypeInfo(typeof(T)).Assembly, optional, reloadOnChange: false);
	}

	public static IConfigurationBuilder AddUserSecrets<T>(this IConfigurationBuilder configuration, bool optional, bool reloadOnChange) where T : class
	{
		return configuration.AddUserSecrets(IntrospectionExtensions.GetTypeInfo(typeof(T)).Assembly, optional, reloadOnChange);
	}

	public static IConfigurationBuilder AddUserSecrets(this IConfigurationBuilder configuration, Assembly assembly)
	{
		return configuration.AddUserSecrets(assembly, optional: false, reloadOnChange: false);
	}

	public static IConfigurationBuilder AddUserSecrets(this IConfigurationBuilder configuration, Assembly assembly, bool optional)
	{
		return configuration.AddUserSecrets(assembly, optional, reloadOnChange: false);
	}

	public static IConfigurationBuilder AddUserSecrets(this IConfigurationBuilder configuration, Assembly assembly, bool optional, bool reloadOnChange)
	{
		if (configuration == null)
		{
			throw new ArgumentNullException("configuration");
		}
		if (assembly == null)
		{
			throw new ArgumentNullException("assembly");
		}
		UserSecretsIdAttribute attribute = CustomAttributeExtensions.GetCustomAttribute<UserSecretsIdAttribute>(assembly);
		if (attribute != null)
		{
			return configuration.AddUserSecrets(attribute.UserSecretsId, reloadOnChange);
		}
		if (!optional)
		{
			throw new InvalidOperationException(SR.Format(SR.Error_Missing_UserSecretsIdAttribute, assembly.GetName().Name));
		}
		return configuration;
	}

	public static IConfigurationBuilder AddUserSecrets(this IConfigurationBuilder configuration, string userSecretsId)
	{
		return configuration.AddUserSecrets(userSecretsId, reloadOnChange: false);
	}

	public static IConfigurationBuilder AddUserSecrets(this IConfigurationBuilder configuration, string userSecretsId, bool reloadOnChange)
	{
		if (configuration == null)
		{
			throw new ArgumentNullException("configuration");
		}
		if (userSecretsId == null)
		{
			throw new ArgumentNullException("userSecretsId");
		}
		return AddSecretsFile(configuration, PathHelper.GetSecretsPathFromSecretsId(userSecretsId), reloadOnChange);
	}

	private static IConfigurationBuilder AddSecretsFile(IConfigurationBuilder configuration, string secretPath, bool reloadOnChange)
	{
		string directoryPath = Path.GetDirectoryName(secretPath);
		PhysicalFileProvider fileProvider = (Directory.Exists(directoryPath) ? new PhysicalFileProvider(directoryPath) : null);
		return configuration.AddJsonFile(fileProvider, "secrets.json", optional: true, reloadOnChange);
	}
}
