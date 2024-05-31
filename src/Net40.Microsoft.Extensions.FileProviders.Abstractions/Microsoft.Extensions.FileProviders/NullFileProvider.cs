using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.FileProviders;

public class NullFileProvider : IFileProvider
{
	public IDirectoryContents GetDirectoryContents(string subpath)
	{
		return NotFoundDirectoryContents.Singleton;
	}

	public IFileInfo GetFileInfo(string subpath)
	{
		return new NotFoundFileInfo(subpath);
	}

	public IChangeToken Watch(string filter)
	{
		return NullChangeToken.Singleton;
	}
}
