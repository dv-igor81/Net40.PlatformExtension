namespace Microsoft.Extensions.Logging;

public interface ISupportExternalScope
{
	void SetScopeProvider(IExternalScopeProvider scopeProvider);
}
