using System;
using System.IO;

namespace Microsoft.Extensions.FileProviders;

public interface IFileInfo
{
	bool Exists { get; }

	long Length { get; }

	string PhysicalPath { get; }

	string Name { get; }

	DateTimeOffset LastModified { get; }

	bool IsDirectory { get; }

	Stream CreateReadStream();
}
