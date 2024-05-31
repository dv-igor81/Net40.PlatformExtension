using System;
using System.IO;

namespace Microsoft.Extensions.FileProviders;

public class NotFoundFileInfo : IFileInfo
{
	public bool Exists => false;

	public bool IsDirectory => false;

	public DateTimeOffset LastModified => DateTimeOffset.MinValue;

	public long Length => -1L;

	public string Name { get; }

	public string PhysicalPath => null;

	public NotFoundFileInfo(string name)
	{
		Name = name;
	}

	public Stream CreateReadStream()
	{
		throw new FileNotFoundException("The file " + Name + " does not exist.");
	}
}
