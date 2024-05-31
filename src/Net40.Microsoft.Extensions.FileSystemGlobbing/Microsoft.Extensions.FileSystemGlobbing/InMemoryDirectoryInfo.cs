using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Microsoft.Extensions.FileSystemGlobbing.Internal;

namespace Microsoft.Extensions.FileSystemGlobbing;

public class InMemoryDirectoryInfo : DirectoryInfoBase
{
	private static readonly char[] DirectorySeparators = new char[2]
	{
		Path.DirectorySeparatorChar,
		Path.AltDirectorySeparatorChar
	};

	private readonly IEnumerable<string> _files;

	public override string FullName { get; }

	public override string Name { get; }

	public override DirectoryInfoBase ParentDirectory => new InMemoryDirectoryInfo(Path.GetDirectoryName(FullName), _files, normalized: true);

	public InMemoryDirectoryInfo(string rootDir, IEnumerable<string> files)
		: this(rootDir, files, normalized: false)
	{
	}

	private InMemoryDirectoryInfo(string rootDir, IEnumerable<string> files, bool normalized)
	{
		if (string.IsNullOrEmpty(rootDir))
		{
			throw new ArgumentNullException("rootDir");
		}
		if (files == null)
		{
			files = new List<string>();
		}
		Name = Path.GetFileName(rootDir);
		if (normalized)
		{
			_files = files;
			FullName = rootDir;
			return;
		}
		List<string> fileList = new List<string>(files.Count());
		foreach (string file in files)
		{
			fileList.Add(Path.GetFullPath(file.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)));
		}
		_files = fileList;
		FullName = Path.GetFullPath(rootDir.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar));
	}

	public override IEnumerable<FileSystemInfoBase> EnumerateFileSystemInfos()
	{
		Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
		foreach (string file in _files)
		{
			if (!IsRootDirectory(FullName, file))
			{
				continue;
			}
			int endPath = file.Length;
			int beginSegment = FullName.Length + 1;
			int endSegment = file.IndexOfAny(DirectorySeparators, beginSegment, endPath - beginSegment);
			if (endSegment == -1)
			{
				yield return new InMemoryFileInfo(file, this);
				continue;
			}
			string name = file.Substring(0, endSegment);
			if (!dict.TryGetValue(name, out var list))
			{
				dict[name] = new List<string> { file };
			}
			else
			{
				list.Add(file);
			}
			list = null;
		}
		foreach (KeyValuePair<string, List<string>> item in dict)
		{
			yield return new InMemoryDirectoryInfo(item.Key, item.Value, normalized: true);
		}
	}

	private bool IsRootDirectory(string rootDir, string filePath)
	{
		if (!filePath.StartsWith(rootDir, StringComparison.Ordinal) || filePath.IndexOf(Path.DirectorySeparatorChar, rootDir.Length) != rootDir.Length)
		{
			return false;
		}
		return true;
	}

	public override DirectoryInfoBase GetDirectory(string path)
	{
		if (string.Equals(path, "..", StringComparison.Ordinal))
		{
			return new InMemoryDirectoryInfo(Path.Combine(FullName, path), _files, normalized: true);
		}
		string normPath = Path.GetFullPath(path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar));
		return new InMemoryDirectoryInfo(normPath, _files, normalized: true);
	}

	public override FileInfoBase GetFile(string path)
	{
		string normPath = Path.GetFullPath(path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar));
		foreach (string file in _files)
		{
			if (string.Equals(file, normPath))
			{
				return new InMemoryFileInfo(file, this);
			}
		}
		return null;
	}
}
