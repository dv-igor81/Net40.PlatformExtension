using System;

namespace Microsoft.Extensions.DependencyInjection.ServiceLookup;

internal interface IServiceProviderEngine : IServiceProvider, IDisposable, IAsyncDisposable
{
	IServiceScope RootScope { get; }

	void ValidateService(ServiceDescriptor descriptor);
}
