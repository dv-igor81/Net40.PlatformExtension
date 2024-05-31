using System;

namespace Microsoft.Extensions.Hosting;

public static class HostingEnvironmentExtensions
{
	public static bool IsDevelopment(this IHostingEnvironment hostingEnvironment)
	{
		if (hostingEnvironment == null)
		{
			throw new ArgumentNullException("hostingEnvironment");
		}
		return hostingEnvironment.IsEnvironment(EnvironmentName.Development);
	}

	public static bool IsStaging(this IHostingEnvironment hostingEnvironment)
	{
		if (hostingEnvironment == null)
		{
			throw new ArgumentNullException("hostingEnvironment");
		}
		return hostingEnvironment.IsEnvironment(EnvironmentName.Staging);
	}

	public static bool IsProduction(this IHostingEnvironment hostingEnvironment)
	{
		if (hostingEnvironment == null)
		{
			throw new ArgumentNullException("hostingEnvironment");
		}
		return hostingEnvironment.IsEnvironment(EnvironmentName.Production);
	}

	public static bool IsEnvironment(this IHostingEnvironment hostingEnvironment, string environmentName)
	{
		if (hostingEnvironment == null)
		{
			throw new ArgumentNullException("hostingEnvironment");
		}
		return string.Equals(hostingEnvironment.EnvironmentName, environmentName, StringComparison.OrdinalIgnoreCase);
	}
}
