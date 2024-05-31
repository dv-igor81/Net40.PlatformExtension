using System;

namespace Microsoft.Extensions.Hosting.Internal;

internal class ConfigureContainerAdapter<TContainerBuilder> : IConfigureContainerAdapter
{
	private Action<HostBuilderContext, TContainerBuilder> _action;

	public ConfigureContainerAdapter(Action<HostBuilderContext, TContainerBuilder> action)
	{
		_action = action ?? throw new ArgumentNullException("action");
	}

	public void ConfigureContainer(HostBuilderContext hostContext, object containerBuilder)
	{
		_action(hostContext, (TContainerBuilder)containerBuilder);
	}
}
