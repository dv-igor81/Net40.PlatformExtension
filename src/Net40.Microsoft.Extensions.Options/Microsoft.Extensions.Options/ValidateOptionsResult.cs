using System.Collections.Generic;

namespace Microsoft.Extensions.Options;

public class ValidateOptionsResult
{
	public static readonly ValidateOptionsResult Skip = new ValidateOptionsResult
	{
		Skipped = true
	};

	public static readonly ValidateOptionsResult Success = new ValidateOptionsResult
	{
		Succeeded = true
	};

	public bool Succeeded { get; protected set; }

	public bool Skipped { get; protected set; }

	public bool Failed { get; protected set; }

	public string FailureMessage { get; protected set; }

	public IEnumerable<string> Failures { get; protected set; }

	public static ValidateOptionsResult Fail(string failureMessage)
	{
		ValidateOptionsResult validateOptionsResult = new ValidateOptionsResult();
		validateOptionsResult.Failed = true;
		validateOptionsResult.FailureMessage = failureMessage;
		validateOptionsResult.Failures = new string[1] { failureMessage };
		return validateOptionsResult;
	}

	public static ValidateOptionsResult Fail(IEnumerable<string> failures)
	{
		return new ValidateOptionsResult
		{
			Failed = true,
			FailureMessage = string.Join("; ", failures),
			Failures = failures
		};
	}
}
