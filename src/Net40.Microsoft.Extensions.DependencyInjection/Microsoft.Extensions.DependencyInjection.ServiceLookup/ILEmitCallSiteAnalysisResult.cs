namespace Microsoft.Extensions.DependencyInjection.ServiceLookup;

internal readonly struct ILEmitCallSiteAnalysisResult
{
	public readonly int Size;

	public readonly bool HasScope;

	public ILEmitCallSiteAnalysisResult(int size)
	{
		this = default(ILEmitCallSiteAnalysisResult);
		Size = size;
	}

	public ILEmitCallSiteAnalysisResult(int size, bool hasScope)
	{
		Size = size;
		HasScope = hasScope;
	}

	public ILEmitCallSiteAnalysisResult Add(in ILEmitCallSiteAnalysisResult other)
	{
		return new ILEmitCallSiteAnalysisResult(Size + other.Size, HasScope | other.HasScope);
	}
}
