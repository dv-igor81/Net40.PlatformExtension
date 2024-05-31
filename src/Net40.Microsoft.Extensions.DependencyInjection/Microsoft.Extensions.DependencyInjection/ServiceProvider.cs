using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.ServiceLookup;

namespace Microsoft.Extensions.DependencyInjection;

public sealed class ServiceProvider : IServiceProvider, IDisposable, IServiceProviderEngineCallback, IAsyncDisposable
{
	private readonly IServiceProviderEngine _engine;

	private readonly CallSiteValidator _callSiteValidator;

	internal ServiceProvider(IEnumerable<ServiceDescriptor> serviceDescriptors, ServiceProviderOptions options)
	{
		IServiceProviderEngineCallback callback = null;
		if (options.ValidateScopes)
		{
			callback = this;
			_callSiteValidator = new CallSiteValidator();
		}
		switch (options.Mode)
		{
		case ServiceProviderMode.Default:
			_engine = new DynamicServiceProviderEngine(serviceDescriptors, callback);
			break;
		case ServiceProviderMode.Dynamic:
			_engine = new DynamicServiceProviderEngine(serviceDescriptors, callback);
			break;
		case ServiceProviderMode.Runtime:
			_engine = new RuntimeServiceProviderEngine(serviceDescriptors, callback);
			break;
		case ServiceProviderMode.Expressions:
			_engine = new ExpressionsServiceProviderEngine(serviceDescriptors, callback);
			break;
		default:
			throw new NotSupportedException("Mode");
		}
		if (!options.ValidateOnBuild)
		{
			return;
		}
		List<Exception> exceptions = null;
		foreach (ServiceDescriptor serviceDescriptor in serviceDescriptors)
		{
			try
			{
				_engine.ValidateService(serviceDescriptor);
			}
			catch (Exception e)
			{
				exceptions = exceptions ?? new List<Exception>();
				exceptions.Add(e);
			}
		}
		if (exceptions != null)
		{
			throw new AggregateException("Some services are not able to be constructed", exceptions.ToArray());
		}
	}

	public object GetService(Type serviceType)
	{
		return _engine.GetService(serviceType);
	}

	public void Dispose()
	{
		_engine.Dispose();
	}

	void IServiceProviderEngineCallback.OnCreate(ServiceCallSite callSite)
	{
		_callSiteValidator.ValidateCallSite(callSite);
	}

	void IServiceProviderEngineCallback.OnResolve(Type serviceType, IServiceScope scope)
	{
		_callSiteValidator.ValidateResolution(serviceType, scope, _engine.RootScope);
	}

	public ValueTask DisposeAsync()
	{
		return _engine.DisposeAsync();
	}
}
