using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.Primitives;

public class StringSegmentComparer : IComparer<StringSegment>, IEqualityComparer<StringSegment>
{
	public static StringSegmentComparer Ordinal { get; } = new StringSegmentComparer(StringComparison.Ordinal, StringComparer.Ordinal);


	public static StringSegmentComparer OrdinalIgnoreCase { get; } = new StringSegmentComparer(StringComparison.OrdinalIgnoreCase, StringComparer.OrdinalIgnoreCase);


	private StringComparison Comparison { get; }

	private StringComparer Comparer { get; }

	private StringSegmentComparer(StringComparison comparison, StringComparer comparer)
	{
		Comparison = comparison;
		Comparer = comparer;
	}

	public int Compare(StringSegment x, StringSegment y)
	{
		return StringSegment.Compare(x, y, Comparison);
	}

	public bool Equals(StringSegment x, StringSegment y)
	{
		return StringSegment.Equals(x, y, Comparison);
	}

	public int GetHashCode(StringSegment obj)
	{
		if (!obj.HasValue)
		{
			return 0;
		}
		return Comparer.GetHashCode(obj.Value);
	}
}
