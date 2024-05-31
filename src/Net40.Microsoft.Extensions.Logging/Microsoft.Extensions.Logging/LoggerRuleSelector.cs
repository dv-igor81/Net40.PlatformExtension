using System;

namespace Microsoft.Extensions.Logging;

internal class LoggerRuleSelector
{
	private static readonly char[] WildcardChar = new char[1] { '*' };

	public void Select(LoggerFilterOptions options, Type providerType, string category, out LogLevel? minLevel, out Func<string, string, LogLevel, bool> filter)
	{
		filter = null;
		minLevel = options.MinLevel;
		string providerAlias = ProviderAliasUtilities.GetAlias(providerType);
		LoggerFilterRule current = null;
		foreach (LoggerFilterRule rule in options.Rules)
		{
			if (IsBetter(rule, current, providerType.FullName, category) || (!string.IsNullOrEmpty(providerAlias) && IsBetter(rule, current, providerAlias, category)))
			{
				current = rule;
			}
		}
		if (current != null)
		{
			filter = current.Filter;
			minLevel = current.LogLevel;
		}
	}

	private static bool IsBetter(LoggerFilterRule rule, LoggerFilterRule current, string logger, string category)
	{
		if (rule.ProviderName != null && rule.ProviderName != logger)
		{
			return false;
		}
		if (rule.CategoryName != null)
		{
			string[] categoryParts = rule.CategoryName.Split(WildcardChar);
			if (categoryParts.Length > 2)
			{
				throw new InvalidOperationException("Only one wildcard character is allowed in category name.");
			}
			string prefix = categoryParts[0];
			string suffix = ((categoryParts.Length > 1) ? categoryParts[1] : string.Empty);
			if (!category.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) || !category.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
		}
		if (current?.ProviderName != null)
		{
			if (rule.ProviderName == null)
			{
				return false;
			}
		}
		else if (rule.ProviderName != null)
		{
			return true;
		}
		if (current?.CategoryName != null)
		{
			if (rule.CategoryName == null)
			{
				return false;
			}
			if (current.CategoryName.Length > rule.CategoryName.Length)
			{
				return false;
			}
		}
		return true;
	}
}
