using System;

namespace Microsoft.Extensions.Primitives;

internal static class ThrowHelper
{
	internal static void ThrowArgumentNullException(ExceptionArgument argument)
	{
		throw new ArgumentNullException(GetArgumentName(argument));
	}

	internal static void ThrowArgumentOutOfRangeException(ExceptionArgument argument)
	{
		throw new ArgumentOutOfRangeException(GetArgumentName(argument));
	}

	internal static void ThrowArgumentException(ExceptionResource resource)
	{
		throw new ArgumentException(GetResourceText(resource));
	}

	internal static void ThrowInvalidOperationException(ExceptionResource resource)
	{
		throw new InvalidOperationException(GetResourceText(resource));
	}

	internal static void ThrowInvalidOperationException(ExceptionResource resource, params object[] args)
	{
		throw new InvalidOperationException(string.Format(GetResourceText(resource), args));
	}

	internal static ArgumentNullException GetArgumentNullException(ExceptionArgument argument)
	{
		return new ArgumentNullException(GetArgumentName(argument));
	}

	internal static ArgumentOutOfRangeException GetArgumentOutOfRangeException(ExceptionArgument argument)
	{
		return new ArgumentOutOfRangeException(GetArgumentName(argument));
	}

	internal static ArgumentException GetArgumentException(ExceptionResource resource)
	{
		return new ArgumentException(GetResourceText(resource));
	}

	private static string GetResourceText(ExceptionResource resource)
	{
		return Resources.ResourceManager.GetString(GetResourceName(resource), Resources.Culture);
	}

	private static string GetArgumentName(ExceptionArgument argument)
	{
		return argument.ToString();
	}

	private static string GetResourceName(ExceptionResource resource)
	{
		return resource.ToString();
	}
}
