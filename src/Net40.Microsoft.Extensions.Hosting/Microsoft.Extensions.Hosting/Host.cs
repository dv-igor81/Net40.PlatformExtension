using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;

namespace Microsoft.Extensions.Hosting;

public static class Host
{
	public static IHostBuilder CreateDefaultBuilder()
	{
		return CreateDefaultBuilder(null);
	}

	public static IHostBuilder CreateDefaultBuilder(string[] args)
	{
		HostBuilder builder = new HostBuilder();
		builder.UseContentRoot(Directory.GetCurrentDirectory());
		builder.ConfigureHostConfiguration(delegate(IConfigurationBuilder config)
		{
			config.AddEnvironmentVariables("DOTNET_");
			if (args != null)
			{
				config.AddCommandLine(args);
			}
		});
		builder.ConfigureAppConfiguration(delegate(HostBuilderContext hostingContext, IConfigurationBuilder config)
		{
			IHostEnvironment hostingEnvironment = hostingContext.HostingEnvironment;
			config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddJsonFile("appsettings." + hostingEnvironment.EnvironmentName + ".json", optional: true, reloadOnChange: true);
			if (hostingEnvironment.IsDevelopment() && !string.IsNullOrEmpty(hostingEnvironment.ApplicationName))
			{
				Assembly assembly = Assembly.Load(new AssemblyName(hostingEnvironment.ApplicationName));
				if (assembly != null)
				{
					config.AddUserSecrets(assembly, optional: true);
				}
			}
			config.AddEnvironmentVariables();
			if (args != null)
			{
				config.AddCommandLine(args);
			}
		}).ConfigureLogging(delegate(HostBuilderContext hostingContext, ILoggingBuilder logging)
		{
			logging.AddFilter<EventLogLoggerProvider>((LogLevel level) => level >= LogLevel.Warning);
			logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
			logging.AddConsole();
			logging.AddDebug();
			logging.AddEventSourceLogger();
			logging.AddEventLog();
		}).UseDefaultServiceProvider(delegate(HostBuilderContext context, ServiceProviderOptions options)
		{
			bool validateOnBuild = (options.ValidateScopes = context.HostingEnvironment.IsDevelopment());
			options.ValidateOnBuild = validateOnBuild;
		});
		return builder;
	}
}
