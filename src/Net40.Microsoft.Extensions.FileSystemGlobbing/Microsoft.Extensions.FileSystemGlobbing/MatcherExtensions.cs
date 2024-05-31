using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace Microsoft.Extensions.FileSystemGlobbing;

public static class MatcherExtensions
{
	public static void AddExcludePatterns(this Matcher matcher, params IEnumerable<string>[] excludePatternsGroups)
	{
		foreach (IEnumerable<string> group in excludePatternsGroups)
		{
			foreach (string pattern in group)
			{
				matcher.AddExclude(pattern);
			}
		}
	}

	public static void AddIncludePatterns(this Matcher matcher, params IEnumerable<string>[] includePatternsGroups)
	{
		foreach (IEnumerable<string> group in includePatternsGroups)
		{
			foreach (string pattern in group)
			{
				matcher.AddInclude(pattern);
			}
		}
	}

	public static IEnumerable<string> GetResultsInFullPath(this Matcher matcher, string directoryPath)
	{
		IEnumerable<FilePatternMatch> matches = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(directoryPath))).Files;
		return matches.Select((FilePatternMatch match) => Path.GetFullPath(Path.Combine(directoryPath, match.Path))).ToArray();
	}

	public static PatternMatchingResult Match(this Matcher matcher, string file)
	{
		return matcher.Match(Directory.GetCurrentDirectory(), new List<string> { file });
	}

	public static PatternMatchingResult Match(this Matcher matcher, string rootDir, string file)
	{
		return matcher.Match(rootDir, new List<string> { file });
	}

	public static PatternMatchingResult Match(this Matcher matcher, IEnumerable<string> files)
	{
		return matcher.Match(Directory.GetCurrentDirectory(), files);
	}

	public static PatternMatchingResult Match(this Matcher matcher, string rootDir, IEnumerable<string> files)
	{
		if (matcher == null)
		{
			throw new ArgumentNullException("matcher");
		}
		return matcher.Execute(new InMemoryDirectoryInfo(rootDir, files));
	}
}
