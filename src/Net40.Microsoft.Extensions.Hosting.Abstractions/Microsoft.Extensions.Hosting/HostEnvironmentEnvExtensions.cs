using System;

namespace Microsoft.Extensions.Hosting;

public static class HostEnvironmentEnvExtensions
{
	public static bool IsDevelopment(this IHostEnvironment hostEnvironment)
	{
		if (hostEnvironment == null)
		{
			throw new ArgumentNullException("hostEnvironment");
		}
		return hostEnvironment.IsEnvironment(Environments.Development);
	}

	public static bool IsStaging(this IHostEnvironment hostEnvironment)
	{
		if (hostEnvironment == null)
		{
			throw new ArgumentNullException("hostEnvironment");
		}
		return hostEnvironment.IsEnvironment(Environments.Staging);
	}

	public static bool IsProduction(this IHostEnvironment hostEnvironment)
	{
		if (hostEnvironment == null)
		{
			throw new ArgumentNullException("hostEnvironment");
		}
		return hostEnvironment.IsEnvironment(Environments.Production);
	}

	public static bool IsEnvironment(this IHostEnvironment hostEnvironment, string environmentName)
	{
		if (hostEnvironment == null)
		{
			throw new ArgumentNullException("hostEnvironment");
		}
		return string.Equals(hostEnvironment.EnvironmentName, environmentName, StringComparison.OrdinalIgnoreCase);
	}
}
