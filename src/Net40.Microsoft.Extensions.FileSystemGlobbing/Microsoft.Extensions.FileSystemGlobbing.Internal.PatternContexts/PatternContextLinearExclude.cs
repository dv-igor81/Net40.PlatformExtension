using System;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace Microsoft.Extensions.FileSystemGlobbing.Internal.PatternContexts;

public class PatternContextLinearExclude : PatternContextLinear
{
	public PatternContextLinearExclude(ILinearPattern pattern)
		: base(pattern)
	{
	}

	public override bool Test(DirectoryInfoBase directory)
	{
		if (IsStackEmpty())
		{
			throw new InvalidOperationException("Can't test directory before entering a directory.");
		}
		if (Frame.IsNotApplicable)
		{
			return false;
		}
		return IsLastSegment() && TestMatchingSegment(directory.Name);
	}
}
