using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.FileProviders.Physical;

namespace Microsoft.Extensions.FileProviders.Internal;

public class PhysicalDirectoryContents : IDirectoryContents, IEnumerable<IFileInfo>, IEnumerable
{
	private IEnumerable<IFileInfo> _entries;

	private readonly string _directory;

	private readonly ExclusionFilters _filters;

	public bool Exists => Directory.Exists(_directory);

	public PhysicalDirectoryContents(string directory)
		: this(directory, ExclusionFilters.Sensitive)
	{
	}

	public PhysicalDirectoryContents(string directory, ExclusionFilters filters)
	{
		_directory = directory ?? throw new ArgumentNullException("directory");
		_filters = filters;
	}

	public IEnumerator<IFileInfo> GetEnumerator()
	{
		EnsureInitialized();
		return _entries.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		EnsureInitialized();
		return _entries.GetEnumerator();
	}

	private void EnsureInitialized()
	{
		try
		{
			_entries = (from info in new DirectoryInfo(_directory).EnumerateFileSystemInfos()
				where !FileSystemInfoHelper.IsExcluded(info, _filters)
				select info).Select((Func<FileSystemInfo, IFileInfo>)delegate(FileSystemInfo info)
			{
				if (info is FileInfo info2)
				{
					return new PhysicalFileInfo(info2);
				}
				if (!(info is DirectoryInfo info3))
				{
					throw new InvalidOperationException("Unexpected type of FileSystemInfo");
				}
				return new PhysicalDirectoryInfo(info3);
			});
		}
		catch (Exception ex) when (ex is DirectoryNotFoundException || ex is IOException)
		{
			_entries = Enumerable.Empty<IFileInfo>();
		}
	}
}
