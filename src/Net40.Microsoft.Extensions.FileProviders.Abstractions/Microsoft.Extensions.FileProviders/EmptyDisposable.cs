using System;

namespace Microsoft.Extensions.FileProviders;

public class EmptyDisposable : IDisposable
{
	public static EmptyDisposable Instance { get; } = new EmptyDisposable();


	private EmptyDisposable()
	{
	}

	public void Dispose()
	{
	}
}
