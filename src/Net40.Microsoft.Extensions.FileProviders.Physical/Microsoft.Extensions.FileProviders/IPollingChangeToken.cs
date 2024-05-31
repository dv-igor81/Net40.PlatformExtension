using System.Threading;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.FileProviders;

internal interface IPollingChangeToken : IChangeToken
{
	CancellationTokenSource CancellationTokenSource { get; }
}
