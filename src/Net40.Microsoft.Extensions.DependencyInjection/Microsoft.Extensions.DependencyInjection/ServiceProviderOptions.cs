namespace Microsoft.Extensions.DependencyInjection;

public class ServiceProviderOptions
{
	internal static readonly ServiceProviderOptions Default = new ServiceProviderOptions();

	public bool ValidateScopes { get; set; }

	public bool ValidateOnBuild { get; set; }

	internal ServiceProviderMode Mode { get; set; } = ServiceProviderMode.Default;

}
