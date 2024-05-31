using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.Configuration.CommandLine;

public class CommandLineConfigurationProvider : ConfigurationProvider
{
	private readonly Dictionary<string, string> _switchMappings;

	protected IEnumerable<string> Args { get; private set; }

	public CommandLineConfigurationProvider(IEnumerable<string> args, IDictionary<string, string> switchMappings = null)
	{
		Args = args ?? throw new ArgumentNullException("args");
		if (switchMappings != null)
		{
			_switchMappings = GetValidatedSwitchMappingsCopy(switchMappings);
		}
	}

	public override void Load()
	{
		Dictionary<string, string> data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		using (IEnumerator<string> enumerator = Args.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				string currentArg = enumerator.Current;
				int keyStartIndex = 0;
				if (currentArg.StartsWith("--"))
				{
					keyStartIndex = 2;
				}
				else if (currentArg.StartsWith("-"))
				{
					keyStartIndex = 1;
				}
				else if (currentArg.StartsWith("/"))
				{
					currentArg = $"--{currentArg.Substring(1)}";
					keyStartIndex = 2;
				}
				int separator = currentArg.IndexOf('=');
				string key;
				string value;
				if (separator < 0)
				{
					if (keyStartIndex == 0)
					{
						continue;
					}
					if (_switchMappings != null && _switchMappings.TryGetValue(currentArg, out var mappedKey))
					{
						key = mappedKey;
					}
					else
					{
						if (keyStartIndex == 1)
						{
							continue;
						}
						key = currentArg.Substring(keyStartIndex);
					}
					string previousKey = enumerator.Current;
					if (!enumerator.MoveNext())
					{
						continue;
					}
					value = enumerator.Current;
				}
				else
				{
					string keySegment = currentArg.Substring(0, separator);
					if (_switchMappings != null && _switchMappings.TryGetValue(keySegment, out var mappedKeySegment))
					{
						key = mappedKeySegment;
					}
					else
					{
						if (keyStartIndex == 1)
						{
							throw new FormatException("Resources.FormatError_ShortSwitchNotDefined(currentArg)");
						}
						key = currentArg.Substring(keyStartIndex, separator - keyStartIndex);
					}
					value = currentArg.Substring(separator + 1);
				}
				data[key] = value;
			}
		}
		base.Data = data;
	}

	private Dictionary<string, string> GetValidatedSwitchMappingsCopy(IDictionary<string, string> switchMappings)
	{
		Dictionary<string, string> switchMappingsCopy = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		foreach (KeyValuePair<string, string> mapping in switchMappings)
		{
			if (!mapping.Key.StartsWith("-") && !mapping.Key.StartsWith("--"))
			{
				throw new ArgumentException("Resources.FormatError_InvalidSwitchMapping(mapping.Key)", "switchMappings");
			}
			if (switchMappingsCopy.ContainsKey(mapping.Key))
			{
				throw new ArgumentException("Resources.FormatError_DuplicatedKeyInSwitchMappings(mapping.Key)", "switchMappings");
			}
			switchMappingsCopy.Add(mapping.Key, mapping.Value);
		}
		return switchMappingsCopy;
	}
}
