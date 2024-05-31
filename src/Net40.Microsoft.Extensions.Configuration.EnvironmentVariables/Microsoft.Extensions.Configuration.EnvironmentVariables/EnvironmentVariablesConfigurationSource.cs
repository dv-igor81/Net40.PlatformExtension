namespace Microsoft.Extensions.Configuration.EnvironmentVariables;

public class EnvironmentVariablesConfigurationSource : IConfigurationSource
{
	public string Prefix { get; set; }

	public IConfigurationProvider Build(IConfigurationBuilder builder)
	{
		return new EnvironmentVariablesConfigurationProvider(Prefix);
	}
}
