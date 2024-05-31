using System;

namespace Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;

public class ParentPathSegment : IPathSegment
{
	private static readonly string LiteralParent = "..";

	public bool CanProduceStem => false;

	public bool Match(string value)
	{
		return string.Equals(LiteralParent, value, StringComparison.Ordinal);
	}
}
