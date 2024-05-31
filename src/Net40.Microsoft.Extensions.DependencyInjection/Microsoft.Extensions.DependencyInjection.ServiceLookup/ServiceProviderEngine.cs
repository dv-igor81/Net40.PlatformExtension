using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection.ServiceLookup;

internal abstract class ServiceProviderEngine : IServiceProviderEngine, IServiceProvider, IDisposable, IAsyncDisposable, IServiceScopeFactory
{
	private readonly IServiceProviderEngineCallback _callback;

	private readonly Func<Type, Func<ServiceProviderEngineScope, object>> _createServiceAccessor;

	private bool _disposed;

	internal ConcurrentDictionary<Type, Func<ServiceProviderEngineScope, object>> RealizedServices { get; }

	internal CallSiteFactory CallSiteFactory { get; }

	protected CallSiteRuntimeResolver RuntimeResolver { get; }

	public ServiceProviderEngineScope Root { get; }

	public IServiceScope RootScope => Root;

	protected ServiceProviderEngine(IEnumerable<ServiceDescriptor> serviceDescriptors, IServiceProviderEngineCallback callback)
	{
		_createServiceAccessor = CreateServiceAccessor;
		_callback = callback;
		Root = new ServiceProviderEngineScope(this);
		RuntimeResolver = new CallSiteRuntimeResolver();
		CallSiteFactory = new CallSiteFactory(serviceDescriptors);
		CallSiteFactory.Add(typeof(IServiceProvider), new ServiceProviderCallSite());
		CallSiteFactory.Add(typeof(IServiceScopeFactory), new ServiceScopeFactoryCallSite());
		RealizedServices = new ConcurrentDictionary<Type, Func<ServiceProviderEngineScope, object>>();
	}

	public void ValidateService(ServiceDescriptor descriptor)
	{
		if (descriptor.ServiceType.IsGenericType && !descriptor.ServiceType.IsConstructedGenericType())
		{
			return;
		}
		try
		{
			ServiceCallSite callSite = CallSiteFactory.GetCallSite(descriptor, new CallSiteChain());
			if (callSite != null)
			{
				_callback?.OnCreate(callSite);
			}
		}
		catch (Exception e)
		{
			throw new InvalidOperationException($"Error while validating the service descriptor '{descriptor}': {e.Message}", e);
		}
	}

	public object GetService(Type serviceType)
	{
		return GetService(serviceType, Root);
	}

	protected abstract Func<ServiceProviderEngineScope, object> RealizeService(ServiceCallSite callSite);

	public void Dispose()
	{
		_disposed = true;
		Root.Dispose();
	}

	public ValueTask DisposeAsync()
	{
		_disposed = true;
		return Root.DisposeAsync();
	}

	internal object GetService(Type serviceType, ServiceProviderEngineScope serviceProviderEngineScope)
	{
		if (_disposed)
		{
			ThrowHelper.ThrowObjectDisposedException();
		}
		Func<ServiceProviderEngineScope, object> realizedService = RealizedServices.GetOrAdd(serviceType, _createServiceAccessor);
		_callback?.OnResolve(serviceType, serviceProviderEngineScope);
		DependencyInjectionEventSource.Log.ServiceResolved(serviceType);
		return realizedService(serviceProviderEngineScope);
	}

	public IServiceScope CreateScope()
	{
		if (_disposed)
		{
			ThrowHelper.ThrowObjectDisposedException();
		}
		return new ServiceProviderEngineScope(this);
	}

	private Func<ServiceProviderEngineScope, object> CreateServiceAccessor(Type serviceType)
	{
		ServiceCallSite callSite = CallSiteFactory.GetCallSite(serviceType, new CallSiteChain());
		if (callSite != null)
		{
			DependencyInjectionEventSource.Log.CallSiteBuilt(serviceType, callSite);
			_callback?.OnCreate(callSite);
			return RealizeService(callSite);
		}
		return (ServiceProviderEngineScope _) => (object)null;
	}
}
