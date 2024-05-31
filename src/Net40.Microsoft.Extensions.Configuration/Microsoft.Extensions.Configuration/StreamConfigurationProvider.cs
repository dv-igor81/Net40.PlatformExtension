using System;
using System.IO;

namespace Microsoft.Extensions.Configuration;

public abstract class StreamConfigurationProvider : ConfigurationProvider
{
	private bool _loaded;

	public StreamConfigurationSource Source { get; }

	public StreamConfigurationProvider(StreamConfigurationSource source)
	{
		Source = source ?? throw new ArgumentNullException("source");
	}

	public abstract void Load(Stream stream);

	public override void Load()
	{
		if (_loaded)
		{
			throw new InvalidOperationException("StreamConfigurationProviders cannot be loaded more than once.");
		}
		Load(Source.Stream);
		_loaded = true;
	}
}
