using System;

namespace Microsoft.Extensions.FileSystemGlobbing.Util;

internal static class StringComparisonHelper
{
	public static StringComparer GetStringComparer(StringComparison comparisonType)
	{
		return comparisonType switch
		{
			StringComparison.CurrentCulture => StringComparer.CurrentCulture, 
			StringComparison.CurrentCultureIgnoreCase => StringComparer.CurrentCultureIgnoreCase, 
			StringComparison.Ordinal => StringComparer.Ordinal, 
			StringComparison.OrdinalIgnoreCase => StringComparer.OrdinalIgnoreCase, 
			StringComparison.InvariantCulture => StringComparer.InvariantCulture, 
			StringComparison.InvariantCultureIgnoreCase => StringComparer.InvariantCultureIgnoreCase, 
			_ => throw new InvalidOperationException($"Unexpected StringComparison type: {comparisonType}"), 
		};
	}
}
