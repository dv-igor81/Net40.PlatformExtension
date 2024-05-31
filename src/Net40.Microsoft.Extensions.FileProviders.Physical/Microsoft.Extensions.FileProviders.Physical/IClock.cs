using System;

namespace Microsoft.Extensions.FileProviders.Physical;

internal interface IClock
{
	DateTime UtcNow { get; }
}
