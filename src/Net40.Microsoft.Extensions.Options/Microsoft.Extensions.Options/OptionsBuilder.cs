using System;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Options;

public class OptionsBuilder<TOptions> where TOptions : class
{
	private const string DefaultValidationFailureMessage = "A validation error has occured.";

	public string Name { get; }

	public IServiceCollection Services { get; }

	public OptionsBuilder(IServiceCollection services, string name)
	{
		if (services == null)
		{
			throw new ArgumentNullException("services");
		}
		Services = services;
		Name = name ?? Options.DefaultName;
	}

	public virtual OptionsBuilder<TOptions> Configure(Action<TOptions> configureOptions)
	{
		if (configureOptions == null)
		{
			throw new ArgumentNullException("configureOptions");
		}
		Services.AddSingleton((IConfigureOptions<TOptions>)new ConfigureNamedOptions<TOptions>(Name, configureOptions));
		return this;
	}

	public virtual OptionsBuilder<TOptions> Configure<TDep>(Action<TOptions, TDep> configureOptions) where TDep : class
	{
		if (configureOptions == null)
		{
			throw new ArgumentNullException("configureOptions");
		}
		Services.AddTransient((Func<IServiceProvider, IConfigureOptions<TOptions>>)((IServiceProvider sp) => new ConfigureNamedOptions<TOptions, TDep>(Name, sp.GetRequiredService<TDep>(), configureOptions)));
		return this;
	}

	public virtual OptionsBuilder<TOptions> Configure<TDep1, TDep2>(Action<TOptions, TDep1, TDep2> configureOptions) where TDep1 : class where TDep2 : class
	{
		if (configureOptions == null)
		{
			throw new ArgumentNullException("configureOptions");
		}
		Services.AddTransient((Func<IServiceProvider, IConfigureOptions<TOptions>>)((IServiceProvider sp) => new ConfigureNamedOptions<TOptions, TDep1, TDep2>(Name, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), configureOptions)));
		return this;
	}

	public virtual OptionsBuilder<TOptions> Configure<TDep1, TDep2, TDep3>(Action<TOptions, TDep1, TDep2, TDep3> configureOptions) where TDep1 : class where TDep2 : class where TDep3 : class
	{
		if (configureOptions == null)
		{
			throw new ArgumentNullException("configureOptions");
		}
		Services.AddTransient((Func<IServiceProvider, IConfigureOptions<TOptions>>)((IServiceProvider sp) => new ConfigureNamedOptions<TOptions, TDep1, TDep2, TDep3>(Name, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), configureOptions)));
		return this;
	}

	public virtual OptionsBuilder<TOptions> Configure<TDep1, TDep2, TDep3, TDep4>(Action<TOptions, TDep1, TDep2, TDep3, TDep4> configureOptions) where TDep1 : class where TDep2 : class where TDep3 : class where TDep4 : class
	{
		if (configureOptions == null)
		{
			throw new ArgumentNullException("configureOptions");
		}
		Services.AddTransient((Func<IServiceProvider, IConfigureOptions<TOptions>>)((IServiceProvider sp) => new ConfigureNamedOptions<TOptions, TDep1, TDep2, TDep3, TDep4>(Name, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), sp.GetRequiredService<TDep4>(), configureOptions)));
		return this;
	}

	public virtual OptionsBuilder<TOptions> Configure<TDep1, TDep2, TDep3, TDep4, TDep5>(Action<TOptions, TDep1, TDep2, TDep3, TDep4, TDep5> configureOptions) where TDep1 : class where TDep2 : class where TDep3 : class where TDep4 : class where TDep5 : class
	{
		if (configureOptions == null)
		{
			throw new ArgumentNullException("configureOptions");
		}
		Services.AddTransient((Func<IServiceProvider, IConfigureOptions<TOptions>>)((IServiceProvider sp) => new ConfigureNamedOptions<TOptions, TDep1, TDep2, TDep3, TDep4, TDep5>(Name, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), sp.GetRequiredService<TDep4>(), sp.GetRequiredService<TDep5>(), configureOptions)));
		return this;
	}

	public virtual OptionsBuilder<TOptions> PostConfigure(Action<TOptions> configureOptions)
	{
		if (configureOptions == null)
		{
			throw new ArgumentNullException("configureOptions");
		}
		Services.AddSingleton((IPostConfigureOptions<TOptions>)new PostConfigureOptions<TOptions>(Name, configureOptions));
		return this;
	}

	public virtual OptionsBuilder<TOptions> PostConfigure<TDep>(Action<TOptions, TDep> configureOptions) where TDep : class
	{
		if (configureOptions == null)
		{
			throw new ArgumentNullException("configureOptions");
		}
		Services.AddTransient((Func<IServiceProvider, IPostConfigureOptions<TOptions>>)((IServiceProvider sp) => new PostConfigureOptions<TOptions, TDep>(Name, sp.GetRequiredService<TDep>(), configureOptions)));
		return this;
	}

	public virtual OptionsBuilder<TOptions> PostConfigure<TDep1, TDep2>(Action<TOptions, TDep1, TDep2> configureOptions) where TDep1 : class where TDep2 : class
	{
		if (configureOptions == null)
		{
			throw new ArgumentNullException("configureOptions");
		}
		Services.AddTransient((Func<IServiceProvider, IPostConfigureOptions<TOptions>>)((IServiceProvider sp) => new PostConfigureOptions<TOptions, TDep1, TDep2>(Name, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), configureOptions)));
		return this;
	}

	public virtual OptionsBuilder<TOptions> PostConfigure<TDep1, TDep2, TDep3>(Action<TOptions, TDep1, TDep2, TDep3> configureOptions) where TDep1 : class where TDep2 : class where TDep3 : class
	{
		if (configureOptions == null)
		{
			throw new ArgumentNullException("configureOptions");
		}
		Services.AddTransient((Func<IServiceProvider, IPostConfigureOptions<TOptions>>)((IServiceProvider sp) => new PostConfigureOptions<TOptions, TDep1, TDep2, TDep3>(Name, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), configureOptions)));
		return this;
	}

	public virtual OptionsBuilder<TOptions> PostConfigure<TDep1, TDep2, TDep3, TDep4>(Action<TOptions, TDep1, TDep2, TDep3, TDep4> configureOptions) where TDep1 : class where TDep2 : class where TDep3 : class where TDep4 : class
	{
		if (configureOptions == null)
		{
			throw new ArgumentNullException("configureOptions");
		}
		Services.AddTransient((Func<IServiceProvider, IPostConfigureOptions<TOptions>>)((IServiceProvider sp) => new PostConfigureOptions<TOptions, TDep1, TDep2, TDep3, TDep4>(Name, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), sp.GetRequiredService<TDep4>(), configureOptions)));
		return this;
	}

	public virtual OptionsBuilder<TOptions> PostConfigure<TDep1, TDep2, TDep3, TDep4, TDep5>(Action<TOptions, TDep1, TDep2, TDep3, TDep4, TDep5> configureOptions) where TDep1 : class where TDep2 : class where TDep3 : class where TDep4 : class where TDep5 : class
	{
		if (configureOptions == null)
		{
			throw new ArgumentNullException("configureOptions");
		}
		Services.AddTransient((Func<IServiceProvider, IPostConfigureOptions<TOptions>>)((IServiceProvider sp) => new PostConfigureOptions<TOptions, TDep1, TDep2, TDep3, TDep4, TDep5>(Name, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), sp.GetRequiredService<TDep4>(), sp.GetRequiredService<TDep5>(), configureOptions)));
		return this;
	}

	public virtual OptionsBuilder<TOptions> Validate(Func<TOptions, bool> validation)
	{
		return Validate(validation, "A validation error has occured.");
	}

	public virtual OptionsBuilder<TOptions> Validate(Func<TOptions, bool> validation, string failureMessage)
	{
		if (validation == null)
		{
			throw new ArgumentNullException("validation");
		}
		Services.AddSingleton((IValidateOptions<TOptions>)new ValidateOptions<TOptions>(Name, validation, failureMessage));
		return this;
	}

	public virtual OptionsBuilder<TOptions> Validate<TDep>(Func<TOptions, TDep, bool> validation)
	{
		return Validate(validation, "A validation error has occured.");
	}

	public virtual OptionsBuilder<TOptions> Validate<TDep>(Func<TOptions, TDep, bool> validation, string failureMessage)
	{
		if (validation == null)
		{
			throw new ArgumentNullException("validation");
		}
		Services.AddTransient((Func<IServiceProvider, IValidateOptions<TOptions>>)((IServiceProvider sp) => new ValidateOptions<TOptions, TDep>(Name, sp.GetRequiredService<TDep>(), validation, failureMessage)));
		return this;
	}

	public virtual OptionsBuilder<TOptions> Validate<TDep1, TDep2>(Func<TOptions, TDep1, TDep2, bool> validation)
	{
		return Validate(validation, "A validation error has occured.");
	}

	public virtual OptionsBuilder<TOptions> Validate<TDep1, TDep2>(Func<TOptions, TDep1, TDep2, bool> validation, string failureMessage)
	{
		if (validation == null)
		{
			throw new ArgumentNullException("validation");
		}
		Services.AddTransient((Func<IServiceProvider, IValidateOptions<TOptions>>)((IServiceProvider sp) => new ValidateOptions<TOptions, TDep1, TDep2>(Name, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), validation, failureMessage)));
		return this;
	}

	public virtual OptionsBuilder<TOptions> Validate<TDep1, TDep2, TDep3>(Func<TOptions, TDep1, TDep2, TDep3, bool> validation)
	{
		return Validate(validation, "A validation error has occured.");
	}

	public virtual OptionsBuilder<TOptions> Validate<TDep1, TDep2, TDep3>(Func<TOptions, TDep1, TDep2, TDep3, bool> validation, string failureMessage)
	{
		if (validation == null)
		{
			throw new ArgumentNullException("validation");
		}
		Services.AddTransient((Func<IServiceProvider, IValidateOptions<TOptions>>)((IServiceProvider sp) => new ValidateOptions<TOptions, TDep1, TDep2, TDep3>(Name, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), validation, failureMessage)));
		return this;
	}

	public virtual OptionsBuilder<TOptions> Validate<TDep1, TDep2, TDep3, TDep4>(Func<TOptions, TDep1, TDep2, TDep3, TDep4, bool> validation)
	{
		return Validate(validation, "A validation error has occured.");
	}

	public virtual OptionsBuilder<TOptions> Validate<TDep1, TDep2, TDep3, TDep4>(Func<TOptions, TDep1, TDep2, TDep3, TDep4, bool> validation, string failureMessage)
	{
		if (validation == null)
		{
			throw new ArgumentNullException("validation");
		}
		Services.AddTransient((Func<IServiceProvider, IValidateOptions<TOptions>>)((IServiceProvider sp) => new ValidateOptions<TOptions, TDep1, TDep2, TDep3, TDep4>(Name, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), sp.GetRequiredService<TDep4>(), validation, failureMessage)));
		return this;
	}

	public virtual OptionsBuilder<TOptions> Validate<TDep1, TDep2, TDep3, TDep4, TDep5>(Func<TOptions, TDep1, TDep2, TDep3, TDep4, TDep5, bool> validation)
	{
		return Validate(validation, "A validation error has occured.");
	}

	public virtual OptionsBuilder<TOptions> Validate<TDep1, TDep2, TDep3, TDep4, TDep5>(Func<TOptions, TDep1, TDep2, TDep3, TDep4, TDep5, bool> validation, string failureMessage)
	{
		if (validation == null)
		{
			throw new ArgumentNullException("validation");
		}
		Services.AddTransient((Func<IServiceProvider, IValidateOptions<TOptions>>)((IServiceProvider sp) => new ValidateOptions<TOptions, TDep1, TDep2, TDep3, TDep4, TDep5>(Name, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), sp.GetRequiredService<TDep4>(), sp.GetRequiredService<TDep5>(), validation, failureMessage)));
		return this;
	}
}
