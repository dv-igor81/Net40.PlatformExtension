using System;
using System.IO;

namespace Microsoft.Extensions.FileProviders.Physical;

internal static class FileSystemInfoHelper
{
	public static bool IsExcluded(FileSystemInfo fileSystemInfo, ExclusionFilters filters)
	{
		if (filters == ExclusionFilters.None)
		{
			return false;
		}
		if (fileSystemInfo.Name.StartsWith(".", StringComparison.Ordinal) && (filters & ExclusionFilters.DotPrefixed) != 0)
		{
			return true;
		}
		if (fileSystemInfo.Exists && (((fileSystemInfo.Attributes & FileAttributes.Hidden) != 0 && (filters & ExclusionFilters.Hidden) != 0) || ((fileSystemInfo.Attributes & FileAttributes.System) != 0 && (filters & ExclusionFilters.System) != 0)))
		{
			return true;
		}
		return false;
	}
}
