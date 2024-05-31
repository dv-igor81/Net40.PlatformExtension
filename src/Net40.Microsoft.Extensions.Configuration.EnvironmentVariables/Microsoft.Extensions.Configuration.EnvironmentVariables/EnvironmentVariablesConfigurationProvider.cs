using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.Configuration.EnvironmentVariables;

public class EnvironmentVariablesConfigurationProvider : ConfigurationProvider
{
	private const string MySqlServerPrefix = "MYSQLCONNSTR_";

	private const string SqlAzureServerPrefix = "SQLAZURECONNSTR_";

	private const string SqlServerPrefix = "SQLCONNSTR_";

	private const string CustomPrefix = "CUSTOMCONNSTR_";

	private const string ConnStrKeyFormat = "ConnectionStrings:{0}";

	private const string ProviderKeyFormat = "ConnectionStrings:{0}_ProviderName";

	private readonly string _prefix;

	public EnvironmentVariablesConfigurationProvider()
		: this(string.Empty)
	{
	}

	public EnvironmentVariablesConfigurationProvider(string prefix)
	{
		_prefix = prefix ?? string.Empty;
	}

	public override void Load()
	{
		Load(Environment.GetEnvironmentVariables());
	}

	internal void Load(IDictionary envVariables)
	{
		Dictionary<string, string> data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		IEnumerable<DictionaryEntry> filteredEnvVariables = from entry in envVariables.Cast<DictionaryEntry>().SelectMany(AzureEnvToAppEnv)
			where ((string)entry.Key).StartsWith(_prefix, StringComparison.OrdinalIgnoreCase)
			select entry;
		foreach (DictionaryEntry envVariable in filteredEnvVariables)
		{
			string key = ((string)envVariable.Key).Substring(_prefix.Length);
			data[key] = (string)envVariable.Value;
		}
		base.Data = data;
	}

	private static string NormalizeKey(string key)
	{
		return key.Replace("__", ConfigurationPath.KeyDelimiter);
	}

	private static IEnumerable<DictionaryEntry> AzureEnvToAppEnv(DictionaryEntry entry)
	{
		string key = (string)entry.Key;
		_ = string.Empty;
		string provider = string.Empty;
		string prefix;
		if (key.StartsWith("MYSQLCONNSTR_", StringComparison.OrdinalIgnoreCase))
		{
			prefix = "MYSQLCONNSTR_";
			provider = "MySql.Data.MySqlClient";
		}
		else if (key.StartsWith("SQLAZURECONNSTR_", StringComparison.OrdinalIgnoreCase))
		{
			prefix = "SQLAZURECONNSTR_";
			provider = "System.Data.SqlClient";
		}
		else if (key.StartsWith("SQLCONNSTR_", StringComparison.OrdinalIgnoreCase))
		{
			prefix = "SQLCONNSTR_";
			provider = "System.Data.SqlClient";
		}
		else
		{
			if (!key.StartsWith("CUSTOMCONNSTR_", StringComparison.OrdinalIgnoreCase))
			{
				entry.Key = NormalizeKey(key);
				yield return entry;
				yield break;
			}
			prefix = "CUSTOMCONNSTR_";
		}
		yield return new DictionaryEntry($"ConnectionStrings:{NormalizeKey(key.Substring(prefix.Length))}", entry.Value);
		if (!string.IsNullOrEmpty(provider))
		{
			yield return new DictionaryEntry($"ConnectionStrings:{NormalizeKey(key.Substring(prefix.Length))}_ProviderName", provider);
		}
	}
}
