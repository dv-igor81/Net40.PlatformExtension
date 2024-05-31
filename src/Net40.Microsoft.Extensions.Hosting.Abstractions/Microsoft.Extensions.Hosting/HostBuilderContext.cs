using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.Hosting;

public class HostBuilderContext
{
	public IHostEnvironment HostingEnvironment { get; set; }

	public IConfiguration Configuration { get; set; }

	public IDictionary<object, object> Properties { get; }

	public HostBuilderContext(IDictionary<object, object> properties)
	{
		Properties = properties ?? throw new ArgumentNullException("properties");
	}
}
