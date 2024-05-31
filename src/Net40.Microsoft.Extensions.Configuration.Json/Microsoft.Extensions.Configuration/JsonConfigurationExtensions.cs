using System;
using System.IO;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.Extensions.Configuration;

public static class JsonConfigurationExtensions
{
	public static IConfigurationBuilder AddJsonFile(this IConfigurationBuilder builder, string path)
	{
		return builder.AddJsonFile(null, path, optional: false, reloadOnChange: false);
	}

	public static IConfigurationBuilder AddJsonFile(this IConfigurationBuilder builder, string path, bool optional)
	{
		return builder.AddJsonFile(null, path, optional, reloadOnChange: false);
	}

	public static IConfigurationBuilder AddJsonFile(this IConfigurationBuilder builder, string path, bool optional, bool reloadOnChange)
	{
		return builder.AddJsonFile(null, path, optional, reloadOnChange);
	}

	public static IConfigurationBuilder AddJsonFile(this IConfigurationBuilder builder, IFileProvider provider, string path, bool optional, bool reloadOnChange)
	{
		if (builder == null)
		{
			throw new ArgumentNullException("builder");
		}
		if (string.IsNullOrEmpty(path))
		{
			throw new ArgumentException("Resources.Error_InvalidFilePath", "path");
		}
		return builder.AddJsonFile(delegate(JsonConfigurationSource s)
		{
			s.FileProvider = provider;
			s.Path = path;
			s.Optional = optional;
			s.ReloadOnChange = reloadOnChange;
			s.ResolveFileProvider();
		});
	}

	public static IConfigurationBuilder AddJsonFile(this IConfigurationBuilder builder, Action<JsonConfigurationSource> configureSource)
	{
		return builder.Add(configureSource);
	}

	public static IConfigurationBuilder AddJsonStream(this IConfigurationBuilder builder, Stream stream)
	{
		if (builder == null)
		{
			throw new ArgumentNullException("builder");
		}
		return builder.Add(delegate(JsonStreamConfigurationSource s)
		{
			s.Stream = stream;
		});
	}
}
