using Microsoft.Extensions.FileProviders;

namespace Microsoft.Extensions.Hosting;

public interface IHostEnvironment
{
	string EnvironmentName { get; set; }

	string ApplicationName { get; set; }

	string ContentRootPath { get; set; }

	IFileProvider ContentRootFileProvider { get; set; }
}
