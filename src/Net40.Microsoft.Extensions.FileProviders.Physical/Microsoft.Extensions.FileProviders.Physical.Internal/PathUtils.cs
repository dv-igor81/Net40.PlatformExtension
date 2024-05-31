using System.IO;
using System.Linq;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.FileProviders.Physical.Internal;

internal static class PathUtils
{
	private static readonly char[] _invalidFileNameChars = (from c in Path.GetInvalidFileNameChars()
		where c != Path.DirectorySeparatorChar && c != Path.AltDirectorySeparatorChar
		select c).ToArray();

	private static readonly char[] _invalidFilterChars = _invalidFileNameChars.Where((char c) => c != '*' && c != '|' && c != '?').ToArray();

	private static readonly char[] _pathSeparators = new char[2]
	{
		Path.DirectorySeparatorChar,
		Path.AltDirectorySeparatorChar
	};

	internal static bool HasInvalidPathChars(string path)
	{
		return path.IndexOfAny(_invalidFileNameChars) != -1;
	}

	internal static bool HasInvalidFilterChars(string path)
	{
		return path.IndexOfAny(_invalidFilterChars) != -1;
	}

	internal static string EnsureTrailingSlash(string path)
	{
		if (!string.IsNullOrEmpty(path) && path[path.Length - 1] != Path.DirectorySeparatorChar)
		{
			return path + Path.DirectorySeparatorChar;
		}
		return path;
	}

	internal static bool PathNavigatesAboveRoot(string path)
	{
		StringTokenizer tokenizer = new StringTokenizer(path, _pathSeparators);
		int depth = 0;
		foreach (StringSegment segment in tokenizer)
		{
			if (segment.Equals(".") || segment.Equals(""))
			{
				continue;
			}
			if (segment.Equals(".."))
			{
				depth--;
				if (depth == -1)
				{
					return true;
				}
			}
			else
			{
				depth++;
			}
		}
		return false;
	}
}
