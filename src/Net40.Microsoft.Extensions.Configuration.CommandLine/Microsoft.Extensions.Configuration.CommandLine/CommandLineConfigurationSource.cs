using System.Collections.Generic;

namespace Microsoft.Extensions.Configuration.CommandLine;

public class CommandLineConfigurationSource : IConfigurationSource
{
	public IDictionary<string, string> SwitchMappings { get; set; }

	public IEnumerable<string> Args { get; set; }

	public IConfigurationProvider Build(IConfigurationBuilder builder)
	{
		return new CommandLineConfigurationProvider(Args, SwitchMappings);
	}
}
