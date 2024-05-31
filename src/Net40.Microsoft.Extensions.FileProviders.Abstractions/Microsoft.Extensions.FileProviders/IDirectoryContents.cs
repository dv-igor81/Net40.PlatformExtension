using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Extensions.FileProviders;

public interface IDirectoryContents : IEnumerable<IFileInfo>, IEnumerable
{
	bool Exists { get; }
}
