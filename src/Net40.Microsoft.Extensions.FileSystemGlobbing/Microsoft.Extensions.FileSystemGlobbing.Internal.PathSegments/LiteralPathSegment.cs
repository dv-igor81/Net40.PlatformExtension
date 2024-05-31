using System;
using Microsoft.Extensions.FileSystemGlobbing.Util;

namespace Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;

public class LiteralPathSegment : IPathSegment
{
	private readonly StringComparison _comparisonType;

	public bool CanProduceStem => false;

	public string Value { get; }

	public LiteralPathSegment(string value, StringComparison comparisonType)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		Value = value;
		_comparisonType = comparisonType;
	}

	public bool Match(string value)
	{
		return string.Equals(Value, value, _comparisonType);
	}

	public override bool Equals(object obj)
	{
		return obj is LiteralPathSegment other && _comparisonType == other._comparisonType && string.Equals(other.Value, Value, _comparisonType);
	}

	public override int GetHashCode()
	{
		return StringComparisonHelper.GetStringComparer(_comparisonType).GetHashCode(Value);
	}
}
