using System;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting.Internal;

internal interface IServiceFactoryAdapter
{
	object CreateBuilder(IServiceCollection services);

	IServiceProvider CreateServiceProvider(object containerBuilder);
}
