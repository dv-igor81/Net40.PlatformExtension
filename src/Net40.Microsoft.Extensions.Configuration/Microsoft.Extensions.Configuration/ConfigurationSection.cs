using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Configuration;

public class ConfigurationSection : IConfigurationSection, IConfiguration
{
	private readonly IConfigurationRoot _root;

	private readonly string _path;

	private string _key;

	public string Path => _path;

	public string Key
	{
		get
		{
			if (_key == null)
			{
				_key = ConfigurationPath.GetSectionKey(_path);
			}
			return _key;
		}
	}

	public string Value
	{
		get
		{
			return _root[Path];
		}
		set
		{
			_root[Path] = value;
		}
	}

	public string this[string key]
	{
		get
		{
			return _root[ConfigurationPath.Combine(Path, key)];
		}
		set
		{
			_root[ConfigurationPath.Combine(Path, key)] = value;
		}
	}

	public ConfigurationSection(IConfigurationRoot root, string path)
	{
		if (root == null)
		{
			throw new ArgumentNullException("root");
		}
		if (path == null)
		{
			throw new ArgumentNullException("path");
		}
		_root = root;
		_path = path;
	}

	public IConfigurationSection GetSection(string key)
	{
		return _root.GetSection(ConfigurationPath.Combine(Path, key));
	}

	public IEnumerable<IConfigurationSection> GetChildren()
	{
		return _root.GetChildrenImplementation(Path);
	}

	public IChangeToken GetReloadToken()
	{
		return _root.GetReloadToken();
	}
}
