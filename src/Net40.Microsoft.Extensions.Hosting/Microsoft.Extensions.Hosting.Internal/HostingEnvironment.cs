using Microsoft.Extensions.FileProviders;

namespace Microsoft.Extensions.Hosting.Internal;

public class HostingEnvironment : IHostingEnvironment, IHostEnvironment
{
	public string EnvironmentName { get; set; }

	public string ApplicationName { get; set; }

	public string ContentRootPath { get; set; }

	public IFileProvider ContentRootFileProvider { get; set; }
}
