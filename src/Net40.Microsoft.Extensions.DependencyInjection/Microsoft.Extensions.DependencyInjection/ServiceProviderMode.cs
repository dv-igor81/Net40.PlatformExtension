namespace Microsoft.Extensions.DependencyInjection;

internal enum ServiceProviderMode
{
	Default,
	Dynamic,
	Runtime,
	Expressions,
	ILEmit
}
