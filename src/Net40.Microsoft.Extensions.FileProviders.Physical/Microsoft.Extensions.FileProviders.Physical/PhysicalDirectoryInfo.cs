using System;
using System.IO;

namespace Microsoft.Extensions.FileProviders.Physical;

public class PhysicalDirectoryInfo : IFileInfo
{
	private readonly DirectoryInfo _info;

	public bool Exists => _info.Exists;

	public long Length => -1L;

	public string PhysicalPath => _info.FullName;

	public string Name => _info.Name;

	public DateTimeOffset LastModified => _info.LastWriteTimeUtc;

	public bool IsDirectory => true;

	public PhysicalDirectoryInfo(DirectoryInfo info)
	{
		_info = info;
	}

	public Stream CreateReadStream()
	{
		throw new InvalidOperationException("Cannot create a stream for a directory.");
	}
}
