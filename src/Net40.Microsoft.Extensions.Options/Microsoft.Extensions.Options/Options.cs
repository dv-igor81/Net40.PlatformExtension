namespace Microsoft.Extensions.Options;

public static class Options
{
	public static readonly string DefaultName = string.Empty;

	public static IOptions<TOptions> Create<TOptions>(TOptions options) where TOptions : class, new()
	{
		return new OptionsWrapper<TOptions>(options);
	}
}
