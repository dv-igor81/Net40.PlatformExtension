using System;

namespace Microsoft.Extensions.FileProviders.Physical;

internal class Clock : IClock
{
	public static readonly Clock Instance = new Clock();

	public DateTime UtcNow => DateTime.UtcNow;

	private Clock()
	{
	}
}
