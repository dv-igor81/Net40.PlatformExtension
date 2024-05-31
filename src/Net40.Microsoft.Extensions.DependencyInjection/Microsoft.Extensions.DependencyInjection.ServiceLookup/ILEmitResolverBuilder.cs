using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Microsoft.Extensions.DependencyInjection.ServiceLookup;

internal sealed class ILEmitResolverBuilder : CallSiteVisitor<ILEmitResolverBuilderContext, object>
{
	private class ILEmitResolverBuilderRuntimeContext
	{
		public IServiceScopeFactory ScopeFactory;

		public object[] Constants;

		public Func<IServiceProvider, object>[] Factories;
	}

	private struct GeneratedMethod
	{
		public Func<ServiceProviderEngineScope, object> Lambda;

		public ILEmitResolverBuilderRuntimeContext Context;

		public DynamicMethod DynamicMethod;
	}

	private static readonly MethodInfo ResolvedServicesGetter = typeof(ServiceProviderEngineScope).GetProperty("ResolvedServices", BindingFlags.Instance | BindingFlags.NonPublic).GetMethod();

	private static readonly FieldInfo FactoriesField = typeof(ILEmitResolverBuilderRuntimeContext).GetField("Factories");

	private static readonly FieldInfo ConstantsField = typeof(ILEmitResolverBuilderRuntimeContext).GetField("Constants");

	private static readonly MethodInfo GetTypeFromHandleMethod = typeof(Type).GetMethod("GetTypeFromHandle");

	private static readonly ConstructorInfo CacheKeyCtor = typeof(ServiceCacheKey).GetConstructors().First();

	private readonly CallSiteRuntimeResolver _runtimeResolver;

	private readonly IServiceScopeFactory _serviceScopeFactory;

	private readonly ServiceProviderEngineScope _rootScope;

	private readonly ConcurrentDictionary<ServiceCacheKey, GeneratedMethod> _scopeResolverCache;

	private readonly Func<ServiceCacheKey, ServiceCallSite, GeneratedMethod> _buildTypeDelegate;

	public ILEmitResolverBuilder(CallSiteRuntimeResolver runtimeResolver, IServiceScopeFactory serviceScopeFactory, ServiceProviderEngineScope rootScope)
	{
		if (runtimeResolver == null)
		{
			throw new ArgumentNullException("runtimeResolver");
		}
		_runtimeResolver = runtimeResolver;
		_serviceScopeFactory = serviceScopeFactory;
		_rootScope = rootScope;
		_scopeResolverCache = new ConcurrentDictionary<ServiceCacheKey, GeneratedMethod>();
		_buildTypeDelegate = (ServiceCacheKey key, ServiceCallSite cs) => BuildTypeNoCache(cs);
	}

	public Func<ServiceProviderEngineScope, object> Build(ServiceCallSite callSite)
	{
		if (callSite.Cache.Location == CallSiteResultCacheLocation.Root)
		{
			object value = _runtimeResolver.Resolve(callSite, _rootScope);
			return (ServiceProviderEngineScope scope) => value;
		}
		return BuildType(callSite).Lambda;
	}

	private GeneratedMethod BuildType(ServiceCallSite callSite)
	{
		if (callSite.Cache.Location == CallSiteResultCacheLocation.Scope)
		{
			return _scopeResolverCache.GetOrAdd(callSite.Cache.Key, (ServiceCacheKey key) => _buildTypeDelegate(key, callSite));
		}
		return BuildTypeNoCache(callSite);
	}

	private GeneratedMethod BuildTypeNoCache(ServiceCallSite callSite)
	{
		DynamicMethod dynamicMethod = new DynamicMethod("ResolveService", MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, typeof(object), new Type[2]
		{
			typeof(ILEmitResolverBuilderRuntimeContext),
			typeof(ServiceProviderEngineScope)
		}, GetType(), skipVisibility: true);
		ILGenerator ilGenerator = dynamicMethod.GetILGenerator(ILEmitCallSiteAnalyzer.Instance.CollectGenerationInfo(callSite).Size);
		ILEmitResolverBuilderRuntimeContext runtimeContext = GenerateMethodBody(callSite, ilGenerator);
		DependencyInjectionEventSource.Log.DynamicMethodBuilt(callSite.ServiceType, ilGenerator.ILOffset);
		GeneratedMethod result = default(GeneratedMethod);
		result.Lambda = (Func<ServiceProviderEngineScope, object>)dynamicMethod.CreateDelegate(typeof(Func<ServiceProviderEngineScope, object>), runtimeContext);
		result.Context = runtimeContext;
		result.DynamicMethod = dynamicMethod;
		return result;
	}

	protected override object VisitDisposeCache(ServiceCallSite transientCallSite, ILEmitResolverBuilderContext argument)
	{
		if (transientCallSite.CaptureDisposable)
		{
			BeginCaptureDisposable(argument);
			VisitCallSiteMain(transientCallSite, argument);
			EndCaptureDisposable(argument);
		}
		else
		{
			VisitCallSiteMain(transientCallSite, argument);
		}
		return null;
	}

	protected override object VisitConstructor(ConstructorCallSite constructorCallSite, ILEmitResolverBuilderContext argument)
	{
		ServiceCallSite[] parameterCallSites = constructorCallSite.ParameterCallSites;
		foreach (ServiceCallSite parameterCallSite in parameterCallSites)
		{
			VisitCallSite(parameterCallSite, argument);
		}
		argument.Generator.Emit(OpCodes.Newobj, constructorCallSite.ConstructorInfo);
		return null;
	}

	protected override object VisitRootCache(ServiceCallSite callSite, ILEmitResolverBuilderContext argument)
	{
		AddConstant(argument, _runtimeResolver.Resolve(callSite, _rootScope));
		return null;
	}

	protected override object VisitScopeCache(ServiceCallSite scopedCallSite, ILEmitResolverBuilderContext argument)
	{
		GeneratedMethod generatedMethod = BuildType(scopedCallSite);
		AddConstant(argument, generatedMethod.Context);
		argument.Generator.Emit(OpCodes.Ldarg_1);
		argument.Generator.Emit(OpCodes.Call, generatedMethod.DynamicMethod);
		return null;
	}

	protected override object VisitConstant(ConstantCallSite constantCallSite, ILEmitResolverBuilderContext argument)
	{
		AddConstant(argument, constantCallSite.DefaultValue);
		return null;
	}

	protected override object VisitServiceProvider(ServiceProviderCallSite serviceProviderCallSite, ILEmitResolverBuilderContext argument)
	{
		argument.Generator.Emit(OpCodes.Ldarg_1);
		return null;
	}

	protected override object VisitServiceScopeFactory(ServiceScopeFactoryCallSite serviceScopeFactoryCallSite, ILEmitResolverBuilderContext argument)
	{
		argument.Generator.Emit(OpCodes.Ldarg_0);
		argument.Generator.Emit(OpCodes.Ldfld, typeof(ILEmitResolverBuilderRuntimeContext).GetField("ScopeFactory"));
		return null;
	}

	protected override object VisitIEnumerable(IEnumerableCallSite enumerableCallSite, ILEmitResolverBuilderContext argument)
	{
		if (enumerableCallSite.ServiceCallSites.Length == 0)
		{
			argument.Generator.Emit(OpCodes.Call, ExpressionResolverBuilder.ArrayEmptyMethodInfo.MakeGenericMethod(enumerableCallSite.ItemType));
		}
		else
		{
			argument.Generator.Emit(OpCodes.Ldc_I4, enumerableCallSite.ServiceCallSites.Length);
			argument.Generator.Emit(OpCodes.Newarr, enumerableCallSite.ItemType);
			for (int i = 0; i < enumerableCallSite.ServiceCallSites.Length; i++)
			{
				argument.Generator.Emit(OpCodes.Dup);
				argument.Generator.Emit(OpCodes.Ldc_I4, i);
				VisitCallSite(enumerableCallSite.ServiceCallSites[i], argument);
				argument.Generator.Emit(OpCodes.Stelem, enumerableCallSite.ItemType);
			}
		}
		return null;
	}

	protected override object VisitFactory(FactoryCallSite factoryCallSite, ILEmitResolverBuilderContext argument)
	{
		if (argument.Factories == null)
		{
			argument.Factories = new List<Func<IServiceProvider, object>>();
		}
		argument.Generator.Emit(OpCodes.Ldarg_0);
		argument.Generator.Emit(OpCodes.Ldfld, FactoriesField);
		argument.Generator.Emit(OpCodes.Ldc_I4, argument.Factories.Count);
		argument.Generator.Emit(OpCodes.Ldelem, typeof(Func<IServiceProvider, object>));
		argument.Generator.Emit(OpCodes.Ldarg_1);
		argument.Generator.Emit(OpCodes.Call, ExpressionResolverBuilder.InvokeFactoryMethodInfo);
		argument.Factories.Add(factoryCallSite.Factory);
		return null;
	}

	private void AddConstant(ILEmitResolverBuilderContext argument, object value)
	{
		if (argument.Constants == null)
		{
			argument.Constants = new List<object>();
		}
		argument.Generator.Emit(OpCodes.Ldarg_0);
		argument.Generator.Emit(OpCodes.Ldfld, ConstantsField);
		argument.Generator.Emit(OpCodes.Ldc_I4, argument.Constants.Count);
		argument.Generator.Emit(OpCodes.Ldelem, typeof(object));
		argument.Constants.Add(value);
	}

	private void AddCacheKey(ILEmitResolverBuilderContext argument, ServiceCacheKey key)
	{
		argument.Generator.Emit(OpCodes.Ldtoken, key.Type);
		argument.Generator.Emit(OpCodes.Call, GetTypeFromHandleMethod);
		argument.Generator.Emit(OpCodes.Ldc_I4, key.Slot);
		argument.Generator.Emit(OpCodes.Newobj, CacheKeyCtor);
	}

	private ILEmitResolverBuilderRuntimeContext GenerateMethodBody(ServiceCallSite callSite, ILGenerator generator)
	{
		ILEmitResolverBuilderContext context = new ILEmitResolverBuilderContext
		{
			Generator = generator,
			Constants = null,
			Factories = null
		};
		if (callSite.Cache.Location == CallSiteResultCacheLocation.Scope)
		{
			LocalBuilder cacheKeyLocal = context.Generator.DeclareLocal(typeof(ServiceCacheKey));
			LocalBuilder resolvedServicesLocal = context.Generator.DeclareLocal(typeof(IDictionary<ServiceCacheKey, object>));
			LocalBuilder lockTakenLocal = context.Generator.DeclareLocal(typeof(bool));
			LocalBuilder resultLocal = context.Generator.DeclareLocal(typeof(object));
			Label skipCreationLabel = context.Generator.DefineLabel();
			Label returnLabel = context.Generator.DefineLabel();
			AddCacheKey(context, callSite.Cache.Key);
			Stloc(context.Generator, cacheKeyLocal.LocalIndex);
			context.Generator.BeginExceptionBlock();
			context.Generator.Emit(OpCodes.Ldarg_1);
			context.Generator.Emit(OpCodes.Callvirt, ResolvedServicesGetter);
			Stloc(context.Generator, resolvedServicesLocal.LocalIndex);
			Ldloc(context.Generator, resolvedServicesLocal.LocalIndex);
			context.Generator.Emit(OpCodes.Ldloca_S, lockTakenLocal.LocalIndex);
			context.Generator.Emit(OpCodes.Call, ExpressionResolverBuilder.MonitorEnterMethodInfo);
			Ldloc(context.Generator, resolvedServicesLocal.LocalIndex);
			Ldloc(context.Generator, cacheKeyLocal.LocalIndex);
			context.Generator.Emit(OpCodes.Ldloca_S, resultLocal.LocalIndex);
			context.Generator.Emit(OpCodes.Callvirt, ExpressionResolverBuilder.TryGetValueMethodInfo);
			context.Generator.Emit(OpCodes.Brtrue, skipCreationLabel);
			VisitCallSiteMain(callSite, context);
			Stloc(context.Generator, resultLocal.LocalIndex);
			if (callSite.CaptureDisposable)
			{
				BeginCaptureDisposable(context);
				Ldloc(context.Generator, resultLocal.LocalIndex);
				EndCaptureDisposable(context);
				generator.Emit(OpCodes.Pop);
			}
			Ldloc(context.Generator, resolvedServicesLocal.LocalIndex);
			Ldloc(context.Generator, cacheKeyLocal.LocalIndex);
			Ldloc(context.Generator, resultLocal.LocalIndex);
			context.Generator.Emit(OpCodes.Callvirt, ExpressionResolverBuilder.AddMethodInfo);
			context.Generator.MarkLabel(skipCreationLabel);
			context.Generator.BeginFinallyBlock();
			Ldloc(context.Generator, lockTakenLocal.LocalIndex);
			context.Generator.Emit(OpCodes.Brfalse, returnLabel);
			Ldloc(context.Generator, resolvedServicesLocal.LocalIndex);
			context.Generator.Emit(OpCodes.Call, ExpressionResolverBuilder.MonitorExitMethodInfo);
			context.Generator.MarkLabel(returnLabel);
			context.Generator.EndExceptionBlock();
			Ldloc(context.Generator, resultLocal.LocalIndex);
			context.Generator.Emit(OpCodes.Ret);
		}
		else
		{
			VisitCallSite(callSite, context);
			context.Generator.Emit(OpCodes.Ret);
		}
		return new ILEmitResolverBuilderRuntimeContext
		{
			Constants = context.Constants?.ToArray(),
			Factories = context.Factories?.ToArray(),
			ScopeFactory = _serviceScopeFactory
		};
	}

	private static void BeginCaptureDisposable(ILEmitResolverBuilderContext argument)
	{
		argument.Generator.Emit(OpCodes.Ldarg_1);
	}

	private static void EndCaptureDisposable(ILEmitResolverBuilderContext argument)
	{
		argument.Generator.Emit(OpCodes.Callvirt, ExpressionResolverBuilder.CaptureDisposableMethodInfo);
	}

	private void Ldloc(ILGenerator generator, int index)
	{
		switch (index)
		{
		case 0:
			generator.Emit(OpCodes.Ldloc_0);
			return;
		case 1:
			generator.Emit(OpCodes.Ldloc_1);
			return;
		case 2:
			generator.Emit(OpCodes.Ldloc_2);
			return;
		case 3:
			generator.Emit(OpCodes.Ldloc_3);
			return;
		}
		if (index < 255)
		{
			generator.Emit(OpCodes.Ldloc_S, (byte)index);
		}
		else
		{
			generator.Emit(OpCodes.Ldloc, index);
		}
	}

	private void Stloc(ILGenerator generator, int index)
	{
		switch (index)
		{
		case 0:
			generator.Emit(OpCodes.Stloc_0);
			return;
		case 1:
			generator.Emit(OpCodes.Stloc_1);
			return;
		case 2:
			generator.Emit(OpCodes.Stloc_2);
			return;
		case 3:
			generator.Emit(OpCodes.Stloc_3);
			return;
		}
		if (index < 255)
		{
			generator.Emit(OpCodes.Stloc_S, (byte)index);
		}
		else
		{
			generator.Emit(OpCodes.Stloc, index);
		}
	}
}
