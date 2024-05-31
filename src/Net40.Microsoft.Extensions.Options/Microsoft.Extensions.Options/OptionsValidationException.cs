using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.Options;

public class OptionsValidationException : Exception
{
	public string OptionsName { get; }

	public Type OptionsType { get; }

	public IEnumerable<string> Failures { get; }

	public override string Message => string.Join("; ", Failures);

	public OptionsValidationException(string optionsName, Type optionsType, IEnumerable<string> failureMessages)
	{
		Failures = failureMessages ?? new List<string>();
		OptionsType = optionsType ?? throw new ArgumentNullException("optionsType");
		OptionsName = optionsName ?? throw new ArgumentNullException("optionsName");
	}
}
