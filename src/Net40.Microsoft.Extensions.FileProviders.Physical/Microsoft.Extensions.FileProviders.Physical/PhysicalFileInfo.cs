using System;
using System.IO;

namespace Microsoft.Extensions.FileProviders.Physical;

public class PhysicalFileInfo : IFileInfo
{
	private readonly FileInfo _info;

	public bool Exists => _info.Exists;

	public long Length => _info.Length;

	public string PhysicalPath => _info.FullName;

	public string Name => _info.Name;

	public DateTimeOffset LastModified => _info.LastWriteTimeUtc;

	public bool IsDirectory => false;

	public PhysicalFileInfo(FileInfo info)
	{
		_info = info;
	}

	public Stream CreateReadStream()
	{
		int bufferSize = 1;
		return new FileStream(PhysicalPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize, FileOptions.Asynchronous | FileOptions.SequentialScan);
	}
}
