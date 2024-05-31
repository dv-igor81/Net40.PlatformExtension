using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.Configuration;

public static class ConfigurationPath
{
	public static readonly string KeyDelimiter = ":";

	public static string Combine(params string[] pathSegments)
	{
		if (pathSegments == null)
		{
			throw new ArgumentNullException("pathSegments");
		}
		return string.Join(KeyDelimiter, pathSegments);
	}

	public static string Combine(IEnumerable<string> pathSegments)
	{
		if (pathSegments == null)
		{
			throw new ArgumentNullException("pathSegments");
		}
		return string.Join(KeyDelimiter, pathSegments);
	}

	public static string GetSectionKey(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return path;
		}
		int num = path.LastIndexOf(KeyDelimiter, StringComparison.OrdinalIgnoreCase);
		if (num != -1)
		{
			return path.Substring(num + 1);
		}
		return path;
	}

	public static string GetParentPath(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return null;
		}
		int num = path.LastIndexOf(KeyDelimiter, StringComparison.OrdinalIgnoreCase);
		if (num != -1)
		{
			return path.Substring(0, num);
		}
		return null;
	}
}
