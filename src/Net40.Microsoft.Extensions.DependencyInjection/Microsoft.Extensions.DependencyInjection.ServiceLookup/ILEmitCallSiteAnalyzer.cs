namespace Microsoft.Extensions.DependencyInjection.ServiceLookup;

internal sealed class ILEmitCallSiteAnalyzer : CallSiteVisitor<object, ILEmitCallSiteAnalysisResult>
{
	private const int ConstructorILSize = 6;

	private const int ScopedILSize = 64;

	private const int ConstantILSize = 4;

	private const int ServiceProviderSize = 1;

	private const int FactoryILSize = 16;

	internal static ILEmitCallSiteAnalyzer Instance { get; } = new ILEmitCallSiteAnalyzer();


	protected override ILEmitCallSiteAnalysisResult VisitDisposeCache(ServiceCallSite transientCallSite, object argument)
	{
		return VisitCallSiteMain(transientCallSite, argument);
	}

	protected override ILEmitCallSiteAnalysisResult VisitConstructor(ConstructorCallSite constructorCallSite, object argument)
	{
		ILEmitCallSiteAnalysisResult result = new ILEmitCallSiteAnalysisResult(6);
		ServiceCallSite[] parameterCallSites = constructorCallSite.ParameterCallSites;
		foreach (ServiceCallSite callSite in parameterCallSites)
		{
			ILEmitCallSiteAnalysisResult other = VisitCallSite(callSite, argument);
			result = result.Add(in other);
		}
		return result;
	}

	protected override ILEmitCallSiteAnalysisResult VisitRootCache(ServiceCallSite singletonCallSite, object argument)
	{
		return VisitCallSiteMain(singletonCallSite, argument);
	}

	protected override ILEmitCallSiteAnalysisResult VisitScopeCache(ServiceCallSite scopedCallSite, object argument)
	{
		ILEmitCallSiteAnalysisResult iLEmitCallSiteAnalysisResult = new ILEmitCallSiteAnalysisResult(64, hasScope: true);
		ILEmitCallSiteAnalysisResult other = VisitCallSiteMain(scopedCallSite, argument);
		return iLEmitCallSiteAnalysisResult.Add(in other);
	}

	protected override ILEmitCallSiteAnalysisResult VisitConstant(ConstantCallSite constantCallSite, object argument)
	{
		return new ILEmitCallSiteAnalysisResult(4);
	}

	protected override ILEmitCallSiteAnalysisResult VisitServiceProvider(ServiceProviderCallSite serviceProviderCallSite, object argument)
	{
		return new ILEmitCallSiteAnalysisResult(1);
	}

	protected override ILEmitCallSiteAnalysisResult VisitServiceScopeFactory(ServiceScopeFactoryCallSite serviceScopeFactoryCallSite, object argument)
	{
		return new ILEmitCallSiteAnalysisResult(4);
	}

	protected override ILEmitCallSiteAnalysisResult VisitIEnumerable(IEnumerableCallSite enumerableCallSite, object argument)
	{
		ILEmitCallSiteAnalysisResult result = new ILEmitCallSiteAnalysisResult(6);
		ServiceCallSite[] serviceCallSites = enumerableCallSite.ServiceCallSites;
		foreach (ServiceCallSite callSite in serviceCallSites)
		{
			ILEmitCallSiteAnalysisResult other = VisitCallSite(callSite, argument);
			result = result.Add(in other);
		}
		return result;
	}

	protected override ILEmitCallSiteAnalysisResult VisitFactory(FactoryCallSite factoryCallSite, object argument)
	{
		return new ILEmitCallSiteAnalysisResult(16);
	}

	public ILEmitCallSiteAnalysisResult CollectGenerationInfo(ServiceCallSite callSite)
	{
		return VisitCallSite(callSite, null);
	}
}
