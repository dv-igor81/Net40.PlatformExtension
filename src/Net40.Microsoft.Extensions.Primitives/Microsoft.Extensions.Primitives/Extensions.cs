using System.Text;

namespace Microsoft.Extensions.Primitives;

public static class Extensions
{
	public static StringBuilder Append(this StringBuilder builder, StringSegment segment)
	{
		return builder.Append(segment.Buffer, segment.Offset, segment.Length);
	}
}
