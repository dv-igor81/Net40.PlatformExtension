using System;

namespace Microsoft.Extensions.Hosting;

public class HostOptions
{
	public TimeSpan ShutdownTimeout { get; set; } = TimeSpan.FromSeconds(5.0);

}
