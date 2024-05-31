namespace Microsoft.Extensions.Configuration.Json;

public class JsonConfigurationSource : FileConfigurationSource
{
	public override IConfigurationProvider Build(IConfigurationBuilder builder)
	{
		EnsureDefaults(builder);
		return new JsonConfigurationProvider(this);
	}
}
