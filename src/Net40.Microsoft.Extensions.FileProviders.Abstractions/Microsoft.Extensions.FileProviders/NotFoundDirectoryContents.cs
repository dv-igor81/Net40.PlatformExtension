using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.FileProviders;

public class NotFoundDirectoryContents : IDirectoryContents, IEnumerable<IFileInfo>, IEnumerable
{
	public static NotFoundDirectoryContents Singleton { get; } = new NotFoundDirectoryContents();


	public bool Exists => false;

	public IEnumerator<IFileInfo> GetEnumerator()
	{
		return Enumerable.Empty<IFileInfo>().GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
