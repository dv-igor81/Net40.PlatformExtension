namespace Microsoft.Extensions.Options;

public class OptionsManager<TOptions> : IOptions<TOptions>, IOptionsSnapshot<TOptions> where TOptions : class, new()
{
	private readonly IOptionsFactory<TOptions> _factory;

	private readonly OptionsCache<TOptions> _cache = new OptionsCache<TOptions>();

	public TOptions Value => Get(Options.DefaultName);

	public OptionsManager(IOptionsFactory<TOptions> factory)
	{
		_factory = factory;
	}

	public virtual TOptions Get(string name)
	{
		name = name ?? Options.DefaultName;
		return _cache.GetOrAdd(name, () => _factory.Create(name));
	}
}
