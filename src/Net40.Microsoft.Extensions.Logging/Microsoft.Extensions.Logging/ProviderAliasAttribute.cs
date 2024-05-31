using System;

namespace Microsoft.Extensions.Logging;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class ProviderAliasAttribute : Attribute
{
	public string Alias { get; }

	public ProviderAliasAttribute(string alias)
	{
		Alias = alias;
	}
}
