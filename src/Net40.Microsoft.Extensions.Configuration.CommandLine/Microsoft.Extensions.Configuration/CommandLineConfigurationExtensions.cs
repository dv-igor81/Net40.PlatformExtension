using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration.CommandLine;

namespace Microsoft.Extensions.Configuration;

public static class CommandLineConfigurationExtensions
{
    public static IConfigurationBuilder AddCommandLine(this IConfigurationBuilder configurationBuilder, string[] args)
    {
        return configurationBuilder.AddCommandLine(args, null);
    }

    private static IConfigurationBuilder AddCommandLine(this IConfigurationBuilder configurationBuilder, string[] args,
        IDictionary<string, string> switchMappings)
    {
        configurationBuilder.Add(new CommandLineConfigurationSource
        {
            Args = args,
            SwitchMappings = switchMappings
        });
        return configurationBuilder;
    }

    public static IConfigurationBuilder AddCommandLine(this IConfigurationBuilder builder,
        Action<CommandLineConfigurationSource> configureSource)
    {
        return builder.Add(configureSource);
    }
}