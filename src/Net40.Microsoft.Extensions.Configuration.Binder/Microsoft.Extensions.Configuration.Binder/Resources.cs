using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Net40.Microsoft.Extensions.Configuration.Binder;

internal static class Resources
{
	private static ResourceManager s_resourceManager;

	private static ResourceManager ResourceManager => s_resourceManager ?? (s_resourceManager = new ResourceManager(typeof(Resources)));

	internal static CultureInfo Culture { get; set; }

	internal static string Error_CannotActivateAbstractOrInterface => GetResourceString("Error_CannotActivateAbstractOrInterface");

	internal static string Error_FailedBinding => GetResourceString("Error_FailedBinding");

	internal static string Error_FailedToActivate => GetResourceString("Error_FailedToActivate");

	internal static string Error_MissingParameterlessConstructor => GetResourceString("Error_MissingParameterlessConstructor");

	internal static string Error_UnsupportedMultidimensionalArray => GetResourceString("Error_UnsupportedMultidimensionalArray");

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	internal static string GetResourceString(string resourceKey, string defaultValue = null)
	{
		return ResourceManager.GetString(resourceKey, Culture);
	}

	private static string GetResourceString(string resourceKey, string[] formatterNames)
	{
		string text = GetResourceString(resourceKey);
		if (formatterNames != null)
		{
			for (int i = 0; i < formatterNames.Length; i++)
			{
				text = text.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
			}
		}
		return text;
	}

	internal static string FormatError_CannotActivateAbstractOrInterface(object p0)
	{
		return string.Format(Culture, GetResourceString("Error_CannotActivateAbstractOrInterface"), new object[1] { p0 });
	}

	internal static string FormatError_FailedBinding(object p0, object p1)
	{
		return string.Format(Culture, GetResourceString("Error_FailedBinding"), new object[2] { p0, p1 });
	}

	internal static string FormatError_FailedToActivate(object p0)
	{
		return string.Format(Culture, GetResourceString("Error_FailedToActivate"), new object[1] { p0 });
	}

	internal static string FormatError_MissingParameterlessConstructor(object p0)
	{
		return string.Format(Culture, GetResourceString("Error_MissingParameterlessConstructor"), new object[1] { p0 });
	}

	internal static string FormatError_UnsupportedMultidimensionalArray(object p0)
	{
		return string.Format(Culture, GetResourceString("Error_UnsupportedMultidimensionalArray"), new object[1] { p0 });
	}
}
