using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Net40.Microsoft.Extensions.Configuration.UserSecrets;

internal static class Resources
{
	private static ResourceManager s_resourceManager;

	internal static ResourceManager ResourceManager => s_resourceManager ?? (s_resourceManager = new ResourceManager(typeof(Resources)));

	internal static CultureInfo Culture { get; set; }

	internal static string Common_StringNullOrEmpty => GetResourceString("Common_StringNullOrEmpty");

	internal static string Error_Invalid_Character_In_UserSecrets_Id => GetResourceString("Error_Invalid_Character_In_UserSecrets_Id");

	internal static string Error_Missing_UserSecretsIdAttribute => GetResourceString("Error_Missing_UserSecretsIdAttribute");

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

	internal static string FormatError_Invalid_Character_In_UserSecrets_Id(object p0, object p1)
	{
		return string.Format(Culture, GetResourceString("Error_Invalid_Character_In_UserSecrets_Id"), new object[2] { p0, p1 });
	}

	internal static string FormatError_Missing_UserSecretsIdAttribute(object p0)
	{
		return string.Format(Culture, GetResourceString("Error_Missing_UserSecretsIdAttribute"), new object[1] { p0 });
	}
}
