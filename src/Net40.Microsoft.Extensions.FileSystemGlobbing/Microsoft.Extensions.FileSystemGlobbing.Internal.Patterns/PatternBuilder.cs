using System;
using System.Collections.Generic;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PatternContexts;

namespace Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns;

public class PatternBuilder
{
	private class LinearPattern : ILinearPattern, IPattern
	{
		public IList<IPathSegment> Segments { get; }

		public LinearPattern(List<IPathSegment> allSegments)
		{
			Segments = allSegments;
		}

		public IPatternContext CreatePatternContextForInclude()
		{
			return new PatternContextLinearInclude(this);
		}

		public IPatternContext CreatePatternContextForExclude()
		{
			return new PatternContextLinearExclude(this);
		}
	}

	private class RaggedPattern : IRaggedPattern, IPattern
	{
		public IList<IList<IPathSegment>> Contains { get; }

		public IList<IPathSegment> EndsWith { get; }

		public IList<IPathSegment> Segments { get; }

		public IList<IPathSegment> StartsWith { get; }

		public RaggedPattern(List<IPathSegment> allSegments, IList<IPathSegment> segmentsPatternStartsWith, IList<IPathSegment> segmentsPatternEndsWith, IList<IList<IPathSegment>> segmentsPatternContains)
		{
			Segments = allSegments;
			StartsWith = segmentsPatternStartsWith;
			Contains = segmentsPatternContains;
			EndsWith = segmentsPatternEndsWith;
		}

		public IPatternContext CreatePatternContextForInclude()
		{
			return new PatternContextRaggedInclude(this);
		}

		public IPatternContext CreatePatternContextForExclude()
		{
			return new PatternContextRaggedExclude(this);
		}
	}

	private static readonly char[] _slashes = new char[2] { '/', '\\' };

	private static readonly char[] _star = new char[1] { '*' };

	public StringComparison ComparisonType { get; }

	public PatternBuilder()
	{
		ComparisonType = StringComparison.OrdinalIgnoreCase;
	}

	public PatternBuilder(StringComparison comparisonType)
	{
		ComparisonType = comparisonType;
	}

	public IPattern Build(string pattern)
	{
		if (pattern == null)
		{
			throw new ArgumentNullException("pattern");
		}
		pattern = pattern.TrimStart(_slashes);
		if (pattern.TrimEnd(_slashes).Length < pattern.Length)
		{
			pattern = pattern.TrimEnd(_slashes) + "/**";
		}
		List<IPathSegment> allSegments = new List<IPathSegment>();
		bool isParentSegmentLegal = true;
		IList<IPathSegment> segmentsPatternStartsWith = null;
		IList<IList<IPathSegment>> segmentsPatternContains = null;
		IList<IPathSegment> segmentsPatternEndsWith = null;
		int endPattern = pattern.Length;
		int scanPattern = 0;
		while (scanPattern < endPattern)
		{
			int beginSegment = scanPattern;
			int endSegment = NextIndex(pattern, _slashes, scanPattern, endPattern);
			IPathSegment segment = null;
			if (segment == null && endSegment - beginSegment == 3 && pattern[beginSegment] == '*' && pattern[beginSegment + 1] == '.' && pattern[beginSegment + 2] == '*')
			{
				beginSegment += 2;
			}
			if (segment == null && endSegment - beginSegment == 2)
			{
				if (pattern[beginSegment] == '*' && pattern[beginSegment + 1] == '*')
				{
					segment = new RecursiveWildcardSegment();
				}
				else if (pattern[beginSegment] == '.' && pattern[beginSegment + 1] == '.')
				{
					if (!isParentSegmentLegal)
					{
						throw new ArgumentException("\"..\" can be only added at the beginning of the pattern.");
					}
					segment = new ParentPathSegment();
				}
			}
			if (segment == null && endSegment - beginSegment == 1 && pattern[beginSegment] == '.')
			{
				segment = new CurrentPathSegment();
			}
			if (segment == null && endSegment - beginSegment > 2 && pattern[beginSegment] == '*' && pattern[beginSegment + 1] == '*' && pattern[beginSegment + 2] == '.')
			{
				segment = new RecursiveWildcardSegment();
				endSegment = beginSegment;
			}
			if (segment == null)
			{
				string beginsWith = string.Empty;
				List<string> contains = new List<string>();
				string endsWith = string.Empty;
				int scanSegment = beginSegment;
				while (scanSegment < endSegment)
				{
					int beginLiteral = scanSegment;
					int endLiteral = NextIndex(pattern, _star, scanSegment, endSegment);
					if (beginLiteral == beginSegment)
					{
						if (endLiteral == endSegment)
						{
							segment = new LiteralPathSegment(Portion(pattern, beginLiteral, endLiteral), ComparisonType);
						}
						else
						{
							beginsWith = Portion(pattern, beginLiteral, endLiteral);
						}
					}
					else if (endLiteral == endSegment)
					{
						endsWith = Portion(pattern, beginLiteral, endLiteral);
					}
					else if (beginLiteral != endLiteral)
					{
						contains.Add(Portion(pattern, beginLiteral, endLiteral));
					}
					scanSegment = endLiteral + 1;
				}
				if (segment == null)
				{
					segment = new WildcardPathSegment(beginsWith, contains, endsWith, ComparisonType);
				}
			}
			if (!(segment is ParentPathSegment))
			{
				isParentSegmentLegal = false;
			}
			if (!(segment is CurrentPathSegment))
			{
				if (segment is RecursiveWildcardSegment)
				{
					if (segmentsPatternStartsWith == null)
					{
						segmentsPatternStartsWith = new List<IPathSegment>(allSegments);
						segmentsPatternEndsWith = new List<IPathSegment>();
						segmentsPatternContains = new List<IList<IPathSegment>>();
					}
					else if (segmentsPatternEndsWith.Count != 0)
					{
						segmentsPatternContains.Add(segmentsPatternEndsWith);
						segmentsPatternEndsWith = new List<IPathSegment>();
					}
				}
				else
				{
					segmentsPatternEndsWith?.Add(segment);
				}
				allSegments.Add(segment);
			}
			scanPattern = endSegment + 1;
		}
		if (segmentsPatternStartsWith == null)
		{
			return new LinearPattern(allSegments);
		}
		return new RaggedPattern(allSegments, segmentsPatternStartsWith, segmentsPatternEndsWith, segmentsPatternContains);
	}

	private static int NextIndex(string pattern, char[] anyOf, int beginIndex, int endIndex)
	{
		int index = pattern.IndexOfAny(anyOf, beginIndex, endIndex - beginIndex);
		return (index == -1) ? endIndex : index;
	}

	private static string Portion(string pattern, int beginIndex, int endIndex)
	{
		return pattern.Substring(beginIndex, endIndex - beginIndex);
	}
}
