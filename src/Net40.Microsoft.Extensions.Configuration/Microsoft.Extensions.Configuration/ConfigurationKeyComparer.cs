using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.Configuration;

public class ConfigurationKeyComparer : IComparer<string>
{
	private static readonly string[] _keyDelimiterArray = new string[1] { ConfigurationPath.KeyDelimiter };

	public static ConfigurationKeyComparer Instance { get; } = new ConfigurationKeyComparer();


	public int Compare(string x, string y)
	{
		string[] xParts = x?.Split(_keyDelimiterArray, StringSplitOptions.RemoveEmptyEntries) ?? ArrayEx.Empty<string>();
		string[] yParts = y?.Split(_keyDelimiterArray, StringSplitOptions.RemoveEmptyEntries) ?? ArrayEx.Empty<string>();
		for (int i = 0; i < Math.Min(xParts.Length, yParts.Length); i++)
		{
			x = xParts[i];
			y = yParts[i];
			int value1 = 0;
			int value2 = 0;
			bool xIsInt = x != null && int.TryParse(x, out value1);
			bool yIsInt = y != null && int.TryParse(y, out value2);
			int result = ((!xIsInt && !yIsInt) ? string.Compare(x, y, StringComparison.OrdinalIgnoreCase) : ((!(xIsInt && yIsInt)) ? ((!xIsInt) ? 1 : (-1)) : (value1 - value2)));
			if (result != 0)
			{
				return result;
			}
		}
		return xParts.Length - yParts.Length;
	}
}
