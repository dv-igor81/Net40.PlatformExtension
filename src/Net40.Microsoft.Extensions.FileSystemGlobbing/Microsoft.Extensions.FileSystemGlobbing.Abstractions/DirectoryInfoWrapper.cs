using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Extensions.FileSystemGlobbing.Abstractions;

public class DirectoryInfoWrapper : DirectoryInfoBase
{
	private readonly DirectoryInfo _directoryInfo;

	private readonly bool _isParentPath;

	public override string Name => _isParentPath ? ".." : _directoryInfo.Name;

	public override string FullName => _directoryInfo.FullName;

	public override DirectoryInfoBase ParentDirectory => new DirectoryInfoWrapper(_directoryInfo.Parent);

	public DirectoryInfoWrapper(DirectoryInfo directoryInfo)
		: this(directoryInfo, isParentPath: false)
	{
	}

	private DirectoryInfoWrapper(DirectoryInfo directoryInfo, bool isParentPath)
	{
		_directoryInfo = directoryInfo;
		_isParentPath = isParentPath;
	}

	public override IEnumerable<FileSystemInfoBase> EnumerateFileSystemInfos()
	{
		if (!_directoryInfo.Exists)
		{
			yield break;
		}
		foreach (FileSystemInfo fileSystemInfo in _directoryInfo.EnumerateFileSystemInfos("*", SearchOption.TopDirectoryOnly))
		{
			if (fileSystemInfo is DirectoryInfo directoryInfo)
			{
				yield return new DirectoryInfoWrapper(directoryInfo);
			}
			else
			{
				yield return new FileInfoWrapper((FileInfo)fileSystemInfo);
			}
		}
	}

	public override DirectoryInfoBase GetDirectory(string name)
	{
		bool isParentPath = string.Equals(name, "..", StringComparison.Ordinal);
		if (isParentPath)
		{
			return new DirectoryInfoWrapper(new DirectoryInfo(Path.Combine(_directoryInfo.FullName, name)), isParentPath);
		}
		DirectoryInfo[] dirs = _directoryInfo.GetDirectories(name);
		if (dirs.Length == 1)
		{
			return new DirectoryInfoWrapper(dirs[0], isParentPath);
		}
		if (dirs.Length == 0)
		{
			return null;
		}
		throw new InvalidOperationException($"More than one sub directories are found under {_directoryInfo.FullName} with name {name}.");
	}

	public override FileInfoBase GetFile(string name)
	{
		return new FileInfoWrapper(new FileInfo(Path.Combine(_directoryInfo.FullName, name)));
	}
}
