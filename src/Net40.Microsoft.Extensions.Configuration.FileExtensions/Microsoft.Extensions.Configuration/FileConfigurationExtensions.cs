using System;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.Extensions.Configuration;

public static class FileConfigurationExtensions
{
	private static string FileProviderKey = "FileProvider";

	private static string FileLoadExceptionHandlerKey = "FileLoadExceptionHandler";

	public static IConfigurationBuilder SetFileProvider(this IConfigurationBuilder builder, IFileProvider fileProvider)
	{
		if (builder == null)
		{
			throw new ArgumentNullException("builder");
		}
		builder.Properties[FileProviderKey] = fileProvider ?? throw new ArgumentNullException("fileProvider");
		return builder;
	}

	public static IFileProvider GetFileProvider(this IConfigurationBuilder builder)
	{
		if (builder == null)
		{
			throw new ArgumentNullException("builder");
		}
		if (builder.Properties.TryGetValue(FileProviderKey, out var provider))
		{
			return provider as IFileProvider;
		}
		return new PhysicalFileProvider(AppContext.BaseDirectory ?? string.Empty);
	}

	public static IConfigurationBuilder SetBasePath(this IConfigurationBuilder builder, string basePath)
	{
		if (builder == null)
		{
			throw new ArgumentNullException("builder");
		}
		if (basePath == null)
		{
			throw new ArgumentNullException("basePath");
		}
		return builder.SetFileProvider(new PhysicalFileProvider(basePath));
	}

	public static IConfigurationBuilder SetFileLoadExceptionHandler(this IConfigurationBuilder builder, Action<FileLoadExceptionContext> handler)
	{
		if (builder == null)
		{
			throw new ArgumentNullException("builder");
		}
		builder.Properties[FileLoadExceptionHandlerKey] = handler;
		return builder;
	}

	public static Action<FileLoadExceptionContext> GetFileLoadExceptionHandler(this IConfigurationBuilder builder)
	{
		if (builder == null)
		{
			throw new ArgumentNullException("builder");
		}
		if (builder.Properties.TryGetValue(FileLoadExceptionHandlerKey, out var handler))
		{
			return handler as Action<FileLoadExceptionContext>;
		}
		return null;
	}
}
