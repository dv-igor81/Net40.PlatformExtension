using System;

namespace Microsoft.Extensions.Options;

public class ValidateOptions<TOptions> : IValidateOptions<TOptions> where TOptions : class
{
	public string Name { get; }

	public Func<TOptions, bool> Validation { get; }

	public string FailureMessage { get; }

	public ValidateOptions(string name, Func<TOptions, bool> validation, string failureMessage)
	{
		Name = name;
		Validation = validation;
		FailureMessage = failureMessage;
	}

	public ValidateOptionsResult Validate(string name, TOptions options)
	{
		if (Name == null || name == Name)
		{
			if ((Validation?.Invoke(options)).Value)
			{
				return ValidateOptionsResult.Success;
			}
			return ValidateOptionsResult.Fail(FailureMessage);
		}
		return ValidateOptionsResult.Skip;
	}
}
public class ValidateOptions<TOptions, TDep> : IValidateOptions<TOptions> where TOptions : class
{
	public string Name { get; }

	public Func<TOptions, TDep, bool> Validation { get; }

	public string FailureMessage { get; }

	public TDep Dependency { get; }

	public ValidateOptions(string name, TDep dependency, Func<TOptions, TDep, bool> validation, string failureMessage)
	{
		Name = name;
		Validation = validation;
		FailureMessage = failureMessage;
		Dependency = dependency;
	}

	public ValidateOptionsResult Validate(string name, TOptions options)
	{
		if (Name == null || name == Name)
		{
			if ((Validation?.Invoke(options, Dependency)).Value)
			{
				return ValidateOptionsResult.Success;
			}
			return ValidateOptionsResult.Fail(FailureMessage);
		}
		return ValidateOptionsResult.Skip;
	}
}
public class ValidateOptions<TOptions, TDep1, TDep2> : IValidateOptions<TOptions> where TOptions : class
{
	public string Name { get; }

	public Func<TOptions, TDep1, TDep2, bool> Validation { get; }

	public string FailureMessage { get; }

	public TDep1 Dependency1 { get; }

	public TDep2 Dependency2 { get; }

	public ValidateOptions(string name, TDep1 dependency1, TDep2 dependency2, Func<TOptions, TDep1, TDep2, bool> validation, string failureMessage)
	{
		Name = name;
		Validation = validation;
		FailureMessage = failureMessage;
		Dependency1 = dependency1;
		Dependency2 = dependency2;
	}

	public ValidateOptionsResult Validate(string name, TOptions options)
	{
		if (Name == null || name == Name)
		{
			if ((Validation?.Invoke(options, Dependency1, Dependency2)).Value)
			{
				return ValidateOptionsResult.Success;
			}
			return ValidateOptionsResult.Fail(FailureMessage);
		}
		return ValidateOptionsResult.Skip;
	}
}
public class ValidateOptions<TOptions, TDep1, TDep2, TDep3> : IValidateOptions<TOptions> where TOptions : class
{
	public string Name { get; }

	public Func<TOptions, TDep1, TDep2, TDep3, bool> Validation { get; }

	public string FailureMessage { get; }

	public TDep1 Dependency1 { get; }

	public TDep2 Dependency2 { get; }

	public TDep3 Dependency3 { get; }

	public ValidateOptions(string name, TDep1 dependency1, TDep2 dependency2, TDep3 dependency3, Func<TOptions, TDep1, TDep2, TDep3, bool> validation, string failureMessage)
	{
		Name = name;
		Validation = validation;
		FailureMessage = failureMessage;
		Dependency1 = dependency1;
		Dependency2 = dependency2;
		Dependency3 = dependency3;
	}

	public ValidateOptionsResult Validate(string name, TOptions options)
	{
		if (Name == null || name == Name)
		{
			if ((Validation?.Invoke(options, Dependency1, Dependency2, Dependency3)).Value)
			{
				return ValidateOptionsResult.Success;
			}
			return ValidateOptionsResult.Fail(FailureMessage);
		}
		return ValidateOptionsResult.Skip;
	}
}
public class ValidateOptions<TOptions, TDep1, TDep2, TDep3, TDep4> : IValidateOptions<TOptions> where TOptions : class
{
	public string Name { get; }

	public Func<TOptions, TDep1, TDep2, TDep3, TDep4, bool> Validation { get; }

	public string FailureMessage { get; }

	public TDep1 Dependency1 { get; }

	public TDep2 Dependency2 { get; }

	public TDep3 Dependency3 { get; }

	public TDep4 Dependency4 { get; }

	public ValidateOptions(string name, TDep1 dependency1, TDep2 dependency2, TDep3 dependency3, TDep4 dependency4, Func<TOptions, TDep1, TDep2, TDep3, TDep4, bool> validation, string failureMessage)
	{
		Name = name;
		Validation = validation;
		FailureMessage = failureMessage;
		Dependency1 = dependency1;
		Dependency2 = dependency2;
		Dependency3 = dependency3;
		Dependency4 = dependency4;
	}

	public ValidateOptionsResult Validate(string name, TOptions options)
	{
		if (Name == null || name == Name)
		{
			if ((Validation?.Invoke(options, Dependency1, Dependency2, Dependency3, Dependency4)).Value)
			{
				return ValidateOptionsResult.Success;
			}
			return ValidateOptionsResult.Fail(FailureMessage);
		}
		return ValidateOptionsResult.Skip;
	}
}
public class ValidateOptions<TOptions, TDep1, TDep2, TDep3, TDep4, TDep5> : IValidateOptions<TOptions> where TOptions : class
{
	public string Name { get; }

	public Func<TOptions, TDep1, TDep2, TDep3, TDep4, TDep5, bool> Validation { get; }

	public string FailureMessage { get; }

	public TDep1 Dependency1 { get; }

	public TDep2 Dependency2 { get; }

	public TDep3 Dependency3 { get; }

	public TDep4 Dependency4 { get; }

	public TDep5 Dependency5 { get; }

	public ValidateOptions(string name, TDep1 dependency1, TDep2 dependency2, TDep3 dependency3, TDep4 dependency4, TDep5 dependency5, Func<TOptions, TDep1, TDep2, TDep3, TDep4, TDep5, bool> validation, string failureMessage)
	{
		Name = name;
		Validation = validation;
		FailureMessage = failureMessage;
		Dependency1 = dependency1;
		Dependency2 = dependency2;
		Dependency3 = dependency3;
		Dependency4 = dependency4;
		Dependency5 = dependency5;
	}

	public ValidateOptionsResult Validate(string name, TOptions options)
	{
		if (Name == null || name == Name)
		{
			if ((Validation?.Invoke(options, Dependency1, Dependency2, Dependency3, Dependency4, Dependency5)).Value)
			{
				return ValidateOptionsResult.Success;
			}
			return ValidateOptionsResult.Fail(FailureMessage);
		}
		return ValidateOptionsResult.Skip;
	}
}
