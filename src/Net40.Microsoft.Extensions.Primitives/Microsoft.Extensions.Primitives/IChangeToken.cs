using System;

namespace Microsoft.Extensions.Primitives;

public interface IChangeToken
{
	bool HasChanged { get; }

	bool ActiveChangeCallbacks { get; }

	IDisposable RegisterChangeCallback(Action<object> callback, object state);
}
