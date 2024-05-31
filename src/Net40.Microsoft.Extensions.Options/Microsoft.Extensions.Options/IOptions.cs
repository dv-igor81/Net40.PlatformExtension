namespace Microsoft.Extensions.Options;

public interface IOptions<out TOptions> where TOptions : class, new()
{
	TOptions Value { get; }
}
