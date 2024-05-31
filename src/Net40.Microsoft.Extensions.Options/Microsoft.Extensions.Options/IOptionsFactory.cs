namespace Microsoft.Extensions.Options;

public interface IOptionsFactory<TOptions> where TOptions : class, new()
{
	TOptions Create(string name);
}
