using System;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.FileProviders;

public class NullChangeToken : IChangeToken
{
	public static NullChangeToken Singleton { get; } = new NullChangeToken();


	public bool HasChanged => false;

	public bool ActiveChangeCallbacks => false;

	private NullChangeToken()
	{
	}

	public IDisposable RegisterChangeCallback(Action<object> callback, object state)
	{
		return EmptyDisposable.Instance;
	}
}
