using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.Extensions.DependencyInjection.Abstractions;

internal static class Resources
{
	private static ResourceManager s_resourceManager;

	internal static ResourceManager ResourceManager => s_resourceManager ?? (s_resourceManager = new ResourceManager(typeof(Resources)));

	internal static CultureInfo Culture { get; set; }

	internal static string AmbiguousConstructorMatch => GetResourceString("AmbiguousConstructorMatch");

	internal static string CannotLocateImplementation => GetResourceString("CannotLocateImplementation");

	internal static string CannotResolveService => GetResourceString("CannotResolveService");

	internal static string NoConstructorMatch => GetResourceString("NoConstructorMatch");

	internal static string NoServiceRegistered => GetResourceString("NoServiceRegistered");

	internal static string TryAddIndistinguishableTypeToEnumerable => GetResourceString("TryAddIndistinguishableTypeToEnumerable");

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

	internal static string FormatAmbiguousConstructorMatch(object p0)
	{
		return string.Format(Culture, GetResourceString("AmbiguousConstructorMatch"), new object[1] { p0 });
	}

	internal static string FormatCannotLocateImplementation(object p0, object p1)
	{
		return string.Format(Culture, GetResourceString("CannotLocateImplementation"), new object[2] { p0, p1 });
	}

	internal static string FormatCannotResolveService(object p0, object p1)
	{
		return string.Format(Culture, GetResourceString("CannotResolveService"), new object[2] { p0, p1 });
	}

	internal static string FormatNoConstructorMatch(object p0)
	{
		return string.Format(Culture, GetResourceString("NoConstructorMatch"), new object[1] { p0 });
	}

	internal static string FormatNoServiceRegistered(object p0)
	{
		return string.Format(Culture, GetResourceString("NoServiceRegistered"), new object[1] { p0 });
	}

	internal static string FormatTryAddIndistinguishableTypeToEnumerable(object p0, object p1)
	{
		return string.Format(Culture, GetResourceString("TryAddIndistinguishableTypeToEnumerable"), new object[2] { p0, p1 });
	}
}
