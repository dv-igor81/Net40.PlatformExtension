using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Extensions.DependencyInjection.ServiceLookup;

internal class ThrowHelper
{
	[MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowObjectDisposedException()
	{
		throw new ObjectDisposedException("IServiceProvider");
	}
}
