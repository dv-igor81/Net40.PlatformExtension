using System;
using System.IO;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.Extensions.Configuration;

public abstract class FileConfigurationSource : IConfigurationSource
{
	public IFileProvider FileProvider { get; set; }

	public string Path { get; set; }

	public bool Optional { get; set; }

	public bool ReloadOnChange { get; set; }

	public int ReloadDelay { get; set; } = 250;


	public Action<FileLoadExceptionContext> OnLoadException { get; set; }

	public abstract IConfigurationProvider Build(IConfigurationBuilder builder);

	public void EnsureDefaults(IConfigurationBuilder builder)
	{
		FileProvider = FileProvider ?? builder.GetFileProvider();
		OnLoadException = OnLoadException ?? builder.GetFileLoadExceptionHandler();
	}

	public void ResolveFileProvider()
	{
		if (FileProvider == null && !string.IsNullOrEmpty(Path) && System.IO.Path.IsPathRooted(Path))
		{
			string directory = System.IO.Path.GetDirectoryName(Path);
			string pathToFile = System.IO.Path.GetFileName(Path);
			while (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
			{
				pathToFile = System.IO.Path.Combine(System.IO.Path.GetFileName(directory), pathToFile);
				directory = System.IO.Path.GetDirectoryName(directory);
			}
			if (Directory.Exists(directory))
			{
				FileProvider = new PhysicalFileProvider(directory);
				Path = pathToFile;
			}
		}
	}
}
