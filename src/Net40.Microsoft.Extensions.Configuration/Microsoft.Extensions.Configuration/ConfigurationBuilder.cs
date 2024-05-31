using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.Configuration;

public class ConfigurationBuilder : IConfigurationBuilder
{
	public IList<IConfigurationSource> Sources { get; } = new List<IConfigurationSource>();


	public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();


	public IConfigurationBuilder Add(IConfigurationSource source)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		Sources.Add(source);
		return this;
	}

	public IConfigurationRoot Build()
	{
		List<IConfigurationProvider> providers = new List<IConfigurationProvider>();
		foreach (IConfigurationSource source in Sources)
		{
			IConfigurationProvider provider = source.Build(this);
			providers.Add(provider);
		}
		return new ConfigurationRoot(providers);
	}
}
