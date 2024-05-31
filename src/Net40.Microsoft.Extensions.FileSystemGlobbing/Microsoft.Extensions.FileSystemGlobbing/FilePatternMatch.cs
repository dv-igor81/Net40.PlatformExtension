using System;
using Microsoft.Extensions.Internal;

namespace Microsoft.Extensions.FileSystemGlobbing;

public struct FilePatternMatch : IEquatable<FilePatternMatch>
{
	public string Path { get; }

	public string Stem { get; }

	public FilePatternMatch(string path, string stem)
	{
		Path = path;
		Stem = stem;
	}

	public bool Equals(FilePatternMatch other)
	{
		return string.Equals(other.Path, Path, StringComparison.OrdinalIgnoreCase) && string.Equals(other.Stem, Stem, StringComparison.OrdinalIgnoreCase);
	}

	public override bool Equals(object obj)
	{
		return Equals((FilePatternMatch)obj);
	}

	public override int GetHashCode()
	{
		HashCodeCombiner hashCodeCombiner = HashCodeCombiner.Start();
		hashCodeCombiner.Add(Path, StringComparer.OrdinalIgnoreCase);
		hashCodeCombiner.Add(Stem, StringComparer.OrdinalIgnoreCase);
		return hashCodeCombiner;
	}
}
