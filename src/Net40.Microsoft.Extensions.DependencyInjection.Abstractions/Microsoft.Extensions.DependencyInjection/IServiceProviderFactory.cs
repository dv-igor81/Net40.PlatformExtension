using System;

namespace Microsoft.Extensions.DependencyInjection;

public interface IServiceProviderFactory<TContainerBuilder>
{
	TContainerBuilder CreateBuilder(IServiceCollection services);

	IServiceProvider CreateServiceProvider(TContainerBuilder containerBuilder);
}
