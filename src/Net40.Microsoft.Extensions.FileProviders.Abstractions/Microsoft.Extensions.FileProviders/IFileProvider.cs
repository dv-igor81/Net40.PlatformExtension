using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.FileProviders;

public interface IFileProvider
{
	IFileInfo GetFileInfo(string subpath);

	IDirectoryContents GetDirectoryContents(string subpath);

	IChangeToken Watch(string filter);
}
