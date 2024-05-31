using System.Collections.Generic;

namespace Microsoft.Extensions.Options;

public class OptionsFactory<TOptions> : IOptionsFactory<TOptions> where TOptions : class, new()
{
	private readonly IEnumerable<IConfigureOptions<TOptions>> _setups;

	private readonly IEnumerable<IPostConfigureOptions<TOptions>> _postConfigures;

	private readonly IEnumerable<IValidateOptions<TOptions>> _validations;

	public OptionsFactory(IEnumerable<IConfigureOptions<TOptions>> setups, IEnumerable<IPostConfigureOptions<TOptions>> postConfigures)
		: this(setups, postConfigures, (IEnumerable<IValidateOptions<TOptions>>)null)
	{
	}

	public OptionsFactory(IEnumerable<IConfigureOptions<TOptions>> setups, IEnumerable<IPostConfigureOptions<TOptions>> postConfigures, IEnumerable<IValidateOptions<TOptions>> validations)
	{
		_setups = setups;
		_postConfigures = postConfigures;
		_validations = validations;
	}

	public TOptions Create(string name)
	{
		TOptions options = new TOptions();
		foreach (IConfigureOptions<TOptions> setup in _setups)
		{
			if (setup is IConfigureNamedOptions<TOptions> namedSetup)
			{
				namedSetup.Configure(name, options);
			}
			else if (name == Options.DefaultName)
			{
				setup.Configure(options);
			}
		}
		foreach (IPostConfigureOptions<TOptions> post in _postConfigures)
		{
			post.PostConfigure(name, options);
		}
		if (_validations != null)
		{
			List<string> failures = new List<string>();
			foreach (IValidateOptions<TOptions> validate in _validations)
			{
				ValidateOptionsResult result = validate.Validate(name, options);
				if (result.Failed)
				{
					failures.AddRange(result.Failures);
				}
			}
			if (failures.Count > 0)
			{
				throw new OptionsValidationException(name, typeof(TOptions), failures);
			}
		}
		return options;
	}
}
