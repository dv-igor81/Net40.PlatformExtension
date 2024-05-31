namespace Microsoft.Extensions.Options;

public class OptionsWrapper<TOptions> : IOptions<TOptions> where TOptions : class, new()
{
	public TOptions Value { get; }

	public OptionsWrapper(TOptions options)
	{
		Value = options;
	}
}
