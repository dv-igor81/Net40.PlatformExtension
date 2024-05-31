using System;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.Extensions.Hosting;

[Obsolete("This type is obsolete and will be removed in a future version. The recommended alternative is Microsoft.Extensions.Hosting.IHostEnvironment.", false)]
public interface IHostingEnvironment
{
	string EnvironmentName { get; set; }

	string ApplicationName { get; set; }

	string ContentRootPath { get; set; }

	IFileProvider ContentRootFileProvider { get; set; }
}
