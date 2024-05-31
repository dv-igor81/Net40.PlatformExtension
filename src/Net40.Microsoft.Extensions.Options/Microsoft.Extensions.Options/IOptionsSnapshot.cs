namespace Microsoft.Extensions.Options;

public interface IOptionsSnapshot<out TOptions> : IOptions<TOptions> where TOptions : class, new()
{
	TOptions Get(string name);
}
