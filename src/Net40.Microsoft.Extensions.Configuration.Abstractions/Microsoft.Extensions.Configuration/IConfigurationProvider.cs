using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Configuration;

public interface IConfigurationProvider
{
	bool TryGet(string key, out string value);

	void Set(string key, string value);

	IChangeToken GetReloadToken();

	void Load();

	IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath);
}
