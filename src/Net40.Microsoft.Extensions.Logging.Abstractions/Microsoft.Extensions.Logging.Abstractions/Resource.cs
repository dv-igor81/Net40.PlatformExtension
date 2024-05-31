using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.Extensions.Logging.Abstractions;

internal static class Resource
{
	private static ResourceManager s_resourceManager;

	internal static ResourceManager ResourceManager => s_resourceManager ?? (s_resourceManager = new ResourceManager(typeof(Resource)));

	internal static CultureInfo Culture { get; set; }

	internal static string UnexpectedNumberOfNamedParameters => GetResourceString("UnexpectedNumberOfNamedParameters");

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	private static string GetResourceString(string resourceKey, string defaultValue = null)
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

	internal static string FormatUnexpectedNumberOfNamedParameters(object p0, object p1, object p2)
	{
		return string.Format(Culture, GetResourceString("UnexpectedNumberOfNamedParameters"), new object[3] { p0, p1, p2 });
	}
}
