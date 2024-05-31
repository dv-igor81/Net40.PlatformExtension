using System;

namespace Microsoft.Extensions.FileProviders.Physical;

[Flags]
public enum ExclusionFilters
{
	Sensitive = 7,
	DotPrefixed = 1,
	Hidden = 2,
	System = 4,
	None = 0
}
