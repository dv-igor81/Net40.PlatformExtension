using System;

namespace Microsoft.Extensions.Configuration;

public class FileLoadExceptionContext
{
	public FileConfigurationProvider Provider { get; set; }

	public Exception Exception { get; set; }

	public bool Ignore { get; set; }
}
