using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.Extensions.Primitives;

internal static class Resources
{
	private static ResourceManager s_resourceManager;

	internal static ResourceManager ResourceManager => s_resourceManager ?? (s_resourceManager = new ResourceManager(typeof(Resources)));

	internal static CultureInfo Culture { get; set; }

	internal static string Argument_InvalidOffsetLength => GetResourceString("Argument_InvalidOffsetLength");

	internal static string Argument_InvalidOffsetLengthStringSegment => GetResourceString("Argument_InvalidOffsetLengthStringSegment");

	internal static string Capacity_CannotChangeAfterWriteStarted => GetResourceString("Capacity_CannotChangeAfterWriteStarted");

	internal static string Capacity_NotEnough => GetResourceString("Capacity_NotEnough");

	internal static string Capacity_NotUsedEntirely => GetResourceString("Capacity_NotUsedEntirely");

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

	internal static string FormatCapacity_NotEnough(object p0, object p1)
	{
		return string.Format(Culture, GetResourceString("Capacity_NotEnough"), new object[2] { p0, p1 });
	}

	internal static string FormatCapacity_NotUsedEntirely(object p0, object p1)
	{
		return string.Format(Culture, GetResourceString("Capacity_NotUsedEntirely"), new object[2] { p0, p1 });
	}
}
