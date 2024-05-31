namespace Microsoft.Extensions.Configuration;

public class ChainedConfigurationSource : IConfigurationSource
{
	public IConfiguration Configuration { get; set; }

	public bool ShouldDisposeConfiguration { get; set; }

	public IConfigurationProvider Build(IConfigurationBuilder builder)
	{
		return new ChainedConfigurationProvider(this);
	}
}
