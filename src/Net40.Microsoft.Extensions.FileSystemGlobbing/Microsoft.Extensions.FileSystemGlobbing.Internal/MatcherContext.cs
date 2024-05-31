using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using Microsoft.Extensions.FileSystemGlobbing.Util;

namespace Microsoft.Extensions.FileSystemGlobbing.Internal;

public class MatcherContext
{
	private readonly DirectoryInfoBase _root;

	private readonly List<IPatternContext> _includePatternContexts;

	private readonly List<IPatternContext> _excludePatternContexts;

	private readonly List<FilePatternMatch> _files;

	private readonly HashSet<string> _declaredLiteralFolderSegmentInString;

	private readonly HashSet<LiteralPathSegment> _declaredLiteralFolderSegments = new HashSet<LiteralPathSegment>();

	private readonly HashSet<LiteralPathSegment> _declaredLiteralFileSegments = new HashSet<LiteralPathSegment>();

	private bool _declaredParentPathSegment;

	private bool _declaredWildcardPathSegment;

	private readonly StringComparison _comparisonType;

	public MatcherContext(IEnumerable<IPattern> includePatterns, IEnumerable<IPattern> excludePatterns, DirectoryInfoBase directoryInfo, StringComparison comparison)
	{
		_root = directoryInfo;
		_files = new List<FilePatternMatch>();
		_comparisonType = comparison;
		_includePatternContexts = includePatterns.Select((IPattern pattern) => pattern.CreatePatternContextForInclude()).ToList();
		_excludePatternContexts = excludePatterns.Select((IPattern pattern) => pattern.CreatePatternContextForExclude()).ToList();
		_declaredLiteralFolderSegmentInString = new HashSet<string>(StringComparisonHelper.GetStringComparer(comparison));
	}

	public PatternMatchingResult Execute()
	{
		_files.Clear();
		Match(_root, null);
		return new PatternMatchingResult(_files, _files.Count > 0);
	}

	private void Match(DirectoryInfoBase directory, string parentRelativePath)
	{
		PushDirectory(directory);
		Declare();
		List<FileSystemInfoBase> entities = new List<FileSystemInfoBase>();
		if (_declaredWildcardPathSegment || _declaredLiteralFileSegments.Any())
		{
			entities.AddRange(directory.EnumerateFileSystemInfos());
		}
		else
		{
			IEnumerable<DirectoryInfoBase> candidates = directory.EnumerateFileSystemInfos().OfType<DirectoryInfoBase>();
			foreach (DirectoryInfoBase candidate in candidates)
			{
				if (_declaredLiteralFolderSegmentInString.Contains(candidate.Name))
				{
					entities.Add(candidate);
				}
			}
		}
		if (_declaredParentPathSegment)
		{
			entities.Add(directory.GetDirectory(".."));
		}
		List<DirectoryInfoBase> subDirectories = new List<DirectoryInfoBase>();
		foreach (FileSystemInfoBase entity in entities)
		{
			if (entity is FileInfoBase fileInfo)
			{
				PatternTestResult result = MatchPatternContexts(fileInfo, (IPatternContext pattern, FileInfoBase file) => pattern.Test(file));
				if (result.IsSuccessful)
				{
					_files.Add(new FilePatternMatch(CombinePath(parentRelativePath, fileInfo.Name), result.Stem));
				}
			}
			else if (entity is DirectoryInfoBase directoryInfo && MatchPatternContexts(directoryInfo, (IPatternContext pattern, DirectoryInfoBase dir) => pattern.Test(dir)))
			{
				subDirectories.Add(directoryInfo);
			}
		}
		foreach (DirectoryInfoBase subDir in subDirectories)
		{
			string relativePath = CombinePath(parentRelativePath, subDir.Name);
			Match(subDir, relativePath);
		}
		PopDirectory();
	}

	private void Declare()
	{
		_declaredLiteralFileSegments.Clear();
		_declaredLiteralFolderSegments.Clear();
		_declaredParentPathSegment = false;
		_declaredWildcardPathSegment = false;
		foreach (IPatternContext include in _includePatternContexts)
		{
			include.Declare(DeclareInclude);
		}
	}

	private void DeclareInclude(IPathSegment patternSegment, bool isLastSegment)
	{
		if (patternSegment is LiteralPathSegment literalSegment)
		{
			if (isLastSegment)
			{
				_declaredLiteralFileSegments.Add(literalSegment);
				return;
			}
			_declaredLiteralFolderSegments.Add(literalSegment);
			_declaredLiteralFolderSegmentInString.Add(literalSegment.Value);
		}
		else if (patternSegment is ParentPathSegment)
		{
			_declaredParentPathSegment = true;
		}
		else if (patternSegment is WildcardPathSegment)
		{
			_declaredWildcardPathSegment = true;
		}
	}

	internal static string CombinePath(string left, string right)
	{
		if (string.IsNullOrEmpty(left))
		{
			return right;
		}
		return $"{left}/{right}";
	}

	private bool MatchPatternContexts<TFileInfoBase>(TFileInfoBase fileinfo, Func<IPatternContext, TFileInfoBase, bool> test)
	{
		return MatchPatternContexts(fileinfo, (IPatternContext ctx, TFileInfoBase file) => test(ctx, file) ? PatternTestResult.Success(string.Empty) : PatternTestResult.Failed).IsSuccessful;
	}

	private PatternTestResult MatchPatternContexts<TFileInfoBase>(TFileInfoBase fileinfo, Func<IPatternContext, TFileInfoBase, PatternTestResult> test)
	{
		PatternTestResult result = PatternTestResult.Failed;
		foreach (IPatternContext context in _includePatternContexts)
		{
			PatternTestResult localResult = test(context, fileinfo);
			if (localResult.IsSuccessful)
			{
				result = localResult;
				break;
			}
		}
		if (!result.IsSuccessful)
		{
			return PatternTestResult.Failed;
		}
		foreach (IPatternContext context2 in _excludePatternContexts)
		{
			if (test(context2, fileinfo).IsSuccessful)
			{
				return PatternTestResult.Failed;
			}
		}
		return result;
	}

	private void PopDirectory()
	{
		foreach (IPatternContext context2 in _excludePatternContexts)
		{
			context2.PopDirectory();
		}
		foreach (IPatternContext context in _includePatternContexts)
		{
			context.PopDirectory();
		}
	}

	private void PushDirectory(DirectoryInfoBase directory)
	{
		foreach (IPatternContext context2 in _includePatternContexts)
		{
			context2.PushDirectory(directory);
		}
		foreach (IPatternContext context in _excludePatternContexts)
		{
			context.PushDirectory(directory);
		}
	}
}
