using System;

namespace Microsoft.Extensions.Logging;

internal readonly struct LoggerInformation
{
	public ILogger Logger { get; }

	public string Category { get; }

	public Type ProviderType { get; }

	public bool ExternalScope { get; }

	public LoggerInformation(ILoggerProvider provider, string category)
	{
		this = default(LoggerInformation);
		ProviderType = provider.GetType();
		Logger = provider.CreateLogger(category);
		Category = category;
		ExternalScope = provider is ISupportExternalScope;
	}
}
