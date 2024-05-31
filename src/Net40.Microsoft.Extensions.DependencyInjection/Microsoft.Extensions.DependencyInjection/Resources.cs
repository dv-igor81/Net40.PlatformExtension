using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Net40.Microsoft.Extensions.DependencyInjection;

internal static class Resources
{
	private static ResourceManager s_resourceManager;

	internal static ResourceManager ResourceManager => s_resourceManager ?? (s_resourceManager = new ResourceManager(typeof(Resources)));

	internal static CultureInfo Culture { get; set; }

	internal static string AmbiguousConstructorException => GetResourceString("AmbiguousConstructorException");

	internal static string CannotResolveService => GetResourceString("CannotResolveService");

	internal static string CircularDependencyException => GetResourceString("CircularDependencyException");

	internal static string UnableToActivateTypeException => GetResourceString("UnableToActivateTypeException");

	internal static string OpenGenericServiceRequiresOpenGenericImplementation => GetResourceString("OpenGenericServiceRequiresOpenGenericImplementation");

	internal static string TypeCannotBeActivated => GetResourceString("TypeCannotBeActivated");

	internal static string NoConstructorMatch => GetResourceString("NoConstructorMatch");

	internal static string ScopedInSingletonException => GetResourceString("ScopedInSingletonException");

	internal static string ScopedResolvedFromRootException => GetResourceString("ScopedResolvedFromRootException");

	internal static string DirectScopedResolvedFromRootException => GetResourceString("DirectScopedResolvedFromRootException");

	internal static string ConstantCantBeConvertedToServiceType => GetResourceString("ConstantCantBeConvertedToServiceType");

	internal static string ImplementationTypeCantBeConvertedToServiceType => GetResourceString("ImplementationTypeCantBeConvertedToServiceType");

	internal static string AsyncDisposableServiceDispose => GetResourceString("AsyncDisposableServiceDispose");

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

	internal static string FormatAmbiguousConstructorException(object p0)
	{
		return string.Format(Culture, GetResourceString("AmbiguousConstructorException"), new object[1] { p0 });
	}

	internal static string FormatCannotResolveService(object p0, object p1)
	{
		return string.Format(Culture, GetResourceString("CannotResolveService"), new object[2] { p0, p1 });
	}

	internal static string FormatCircularDependencyException(object p0)
	{
		return string.Format(Culture, GetResourceString("CircularDependencyException"), new object[1] { p0 });
	}

	internal static string FormatUnableToActivateTypeException(object p0)
	{
		return string.Format(Culture, GetResourceString("UnableToActivateTypeException"), new object[1] { p0 });
	}

	internal static string FormatOpenGenericServiceRequiresOpenGenericImplementation(object p0)
	{
		return string.Format(Culture, GetResourceString("OpenGenericServiceRequiresOpenGenericImplementation"), new object[1] { p0 });
	}

	internal static string FormatTypeCannotBeActivated(object p0, object p1)
	{
		return string.Format(Culture, GetResourceString("TypeCannotBeActivated"), new object[2] { p0, p1 });
	}

	internal static string FormatNoConstructorMatch(object p0)
	{
		return string.Format(Culture, GetResourceString("NoConstructorMatch"), new object[1] { p0 });
	}

	internal static string FormatScopedInSingletonException(object p0, object p1, object p2, object p3)
	{
		return string.Format(Culture, GetResourceString("ScopedInSingletonException"), p0, p1, p2, p3);
	}

	internal static string FormatScopedResolvedFromRootException(object p0, object p1, object p2)
	{
		return string.Format(Culture, GetResourceString("ScopedResolvedFromRootException"), new object[3] { p0, p1, p2 });
	}

	internal static string FormatDirectScopedResolvedFromRootException(object p0, object p1)
	{
		return string.Format(Culture, GetResourceString("DirectScopedResolvedFromRootException"), new object[2] { p0, p1 });
	}

	internal static string FormatConstantCantBeConvertedToServiceType(object p0, object p1)
	{
		return string.Format(Culture, GetResourceString("ConstantCantBeConvertedToServiceType"), new object[2] { p0, p1 });
	}

	internal static string FormatImplementationTypeCantBeConvertedToServiceType(object p0, object p1)
	{
		return string.Format(Culture, GetResourceString("ImplementationTypeCantBeConvertedToServiceType"), new object[2] { p0, p1 });
	}

	internal static string FormatAsyncDisposableServiceDispose(object p0)
	{
		return string.Format(Culture, GetResourceString("AsyncDisposableServiceDispose"), new object[1] { p0 });
	}
}
