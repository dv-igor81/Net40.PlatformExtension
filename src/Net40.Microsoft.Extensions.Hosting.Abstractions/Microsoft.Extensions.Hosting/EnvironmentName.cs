using System;

namespace Microsoft.Extensions.Hosting;

[Obsolete("This type is obsolete and will be removed in a future version. The recommended alternative is Microsoft.Extensions.Hosting.Environments.", false)]
public static class EnvironmentName
{
	public static readonly string Development = "Development";

	public static readonly string Staging = "Staging";

	public static readonly string Production = "Production";
}
