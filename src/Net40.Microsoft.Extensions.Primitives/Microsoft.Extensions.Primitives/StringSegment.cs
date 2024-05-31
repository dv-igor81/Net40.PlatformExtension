using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Extensions.Primitives;

public readonly struct StringSegment : IEquatable<StringSegment>, IEquatable<string>
{
	public static readonly StringSegment Empty = string.Empty;

	public string Buffer { get; }

	public int Offset { get; }

	public int Length { get; }

	public string Value
	{
		get
		{
			if (HasValue)
			{
				return Buffer.Substring(Offset, Length);
			}
			return null;
		}
	}

	public bool HasValue => Buffer != null;

	public char this[int index]
	{
		get
		{
			if ((uint)index >= (uint)Length)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
			}
			return Buffer[Offset + index];
		}
	}

	public StringSegment(string buffer)
	{
		Buffer = buffer;
		Offset = 0;
		Length = buffer?.Length ?? 0;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public StringSegment(string buffer, int offset, int length)
	{
		if (buffer == null || (uint)offset > (uint)buffer.Length || (uint)length > (uint)(buffer.Length - offset))
		{
			ThrowInvalidArguments(buffer, offset, length);
		}
		Buffer = buffer;
		Offset = offset;
		Length = length;
	}

	public ReadOnlySpan<char> AsSpan()
	{
		return Buffer.AsSpan(Offset, Length);
	}

	public ReadOnlyMemory<char> AsMemory()
	{
		return Buffer.AsMemory(Offset, Length);
	}

	public static int Compare(StringSegment a, StringSegment b, StringComparison comparisonType)
	{
		int length = Math.Min(a.Length, b.Length);
		int num = string.Compare(a.Buffer, a.Offset, b.Buffer, b.Offset, length, comparisonType);
		if (num == 0)
		{
			num = a.Length - b.Length;
		}
		return num;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (obj is StringSegment other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(StringSegment other)
	{
		return Equals(other, StringComparison.Ordinal);
	}

	public bool Equals(StringSegment other, StringComparison comparisonType)
	{
		if (Length != other.Length)
		{
			return false;
		}
		return string.Compare(Buffer, Offset, other.Buffer, other.Offset, other.Length, comparisonType) == 0;
	}

	public static bool Equals(StringSegment a, StringSegment b, StringComparison comparisonType)
	{
		return a.Equals(b, comparisonType);
	}

	public bool Equals(string text)
	{
		return Equals(text, StringComparison.Ordinal);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public bool Equals(string text, StringComparison comparisonType)
	{
		if (text == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
		}
		int length = text.Length;
		if (!HasValue || Length != length)
		{
			return false;
		}
		return string.Compare(Buffer, Offset, text, 0, length, comparisonType) == 0;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public override int GetHashCode()
	{
		return Value?.GetHashCode() ?? 0;
	}

	public static bool operator ==(StringSegment left, StringSegment right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(StringSegment left, StringSegment right)
	{
		return !left.Equals(right);
	}

	public static implicit operator StringSegment(string value)
	{
		return new StringSegment(value);
	}

	public static implicit operator ReadOnlySpan<char>(StringSegment segment)
	{
		return segment.AsSpan();
	}

	public static implicit operator ReadOnlyMemory<char>(StringSegment segment)
	{
		return segment.AsMemory();
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public bool StartsWith(string text, StringComparison comparisonType)
	{
		if (text == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
		}
		bool result = false;
		int length = text.Length;
		if (HasValue && Length >= length)
		{
			result = string.Compare(Buffer, Offset, text, 0, length, comparisonType) == 0;
		}
		return result;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public bool EndsWith(string text, StringComparison comparisonType)
	{
		if (text == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
		}
		bool result = false;
		int length = text.Length;
		int num = Offset + Length - length;
		if (HasValue && num > 0)
		{
			result = string.Compare(Buffer, num, text, 0, length, comparisonType) == 0;
		}
		return result;
	}

	public string Substring(int offset)
	{
		return Substring(offset, Length - offset);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public string Substring(int offset, int length)
	{
		if (!HasValue || offset < 0 || length < 0 || (uint)(offset + length) > (uint)Length)
		{
			ThrowInvalidArguments(offset, length);
		}
		return Buffer.Substring(Offset + offset, length);
	}

	public StringSegment Subsegment(int offset)
	{
		return Subsegment(offset, Length - offset);
	}

	public StringSegment Subsegment(int offset, int length)
	{
		if (!HasValue || offset < 0 || length < 0 || (uint)(offset + length) > (uint)Length)
		{
			ThrowInvalidArguments(offset, length);
		}
		return new StringSegment(Buffer, Offset + offset, length);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public int IndexOf(char c, int start, int count)
	{
		int num = Offset + start;
		if (!HasValue || start < 0 || (uint)num > (uint)Buffer.Length)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
		}
		if (count < 0)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count);
		}
		int num2 = Buffer.IndexOf(c, num, count);
		if (num2 != -1)
		{
			num2 -= Offset;
		}
		return num2;
	}

	public int IndexOf(char c, int start)
	{
		return IndexOf(c, start, Length - start);
	}

	public int IndexOf(char c)
	{
		return IndexOf(c, 0, Length);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public int IndexOfAny(char[] anyOf, int startIndex, int count)
	{
		int num = -1;
		if (HasValue)
		{
			if (startIndex < 0 || Offset + startIndex > Buffer.Length)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			if (count < 0 || Offset + startIndex + count > Buffer.Length)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count);
			}
			num = Buffer.IndexOfAny(anyOf, Offset + startIndex, count);
			if (num != -1)
			{
				num -= Offset;
			}
		}
		return num;
	}

	public int IndexOfAny(char[] anyOf, int startIndex)
	{
		return IndexOfAny(anyOf, startIndex, Length - startIndex);
	}

	public int IndexOfAny(char[] anyOf)
	{
		return IndexOfAny(anyOf, 0, Length);
	}

	public int LastIndexOf(char value)
	{
		int num = -1;
		if (HasValue)
		{
			num = Buffer.LastIndexOf(value, Offset + Length - 1, Length);
			if (num != -1)
			{
				num -= Offset;
			}
		}
		return num;
	}

	public StringSegment Trim()
	{
		return TrimStart().TrimEnd();
	}

	public unsafe StringSegment TrimStart()
	{
		int i = Offset;
		int num = Offset + Length;
		fixed (char* ptr = Buffer)
		{
			for (; i < num && char.IsWhiteSpace(ptr[i]); i++)
			{
			}
		}
		return new StringSegment(Buffer, i, num - i);
	}

	public unsafe StringSegment TrimEnd()
	{
		int offset = Offset;
		int num = offset + Length - 1;
		fixed (char* ptr = Buffer)
		{
			while (num >= offset && char.IsWhiteSpace(ptr[num]))
			{
				num--;
			}
		}
		return new StringSegment(Buffer, offset, num - offset + 1);
	}

	public StringTokenizer Split(char[] chars)
	{
		return new StringTokenizer(this, chars);
	}

	public static bool IsNullOrEmpty(StringSegment value)
	{
		bool result = false;
		if (!value.HasValue || value.Length == 0)
		{
			result = true;
		}
		return result;
	}

	public override string ToString()
	{
		return Value ?? string.Empty;
	}

	private static void ThrowInvalidArguments(string buffer, int offset, int length)
	{
		throw GetInvalidArgumentsException();
		Exception GetInvalidArgumentsException()
		{
			if (buffer == null)
			{
				return ThrowHelper.GetArgumentNullException(ExceptionArgument.buffer);
			}
			if (offset < 0)
			{
				return ThrowHelper.GetArgumentOutOfRangeException(ExceptionArgument.offset);
			}
			if (length < 0)
			{
				return ThrowHelper.GetArgumentOutOfRangeException(ExceptionArgument.length);
			}
			return ThrowHelper.GetArgumentException(ExceptionResource.Argument_InvalidOffsetLength);
		}
	}

	private void ThrowInvalidArguments(int offset, int length)
	{
		throw GetInvalidArgumentsException(HasValue);
		Exception GetInvalidArgumentsException(bool hasValue)
		{
			if (!hasValue)
			{
				return ThrowHelper.GetArgumentOutOfRangeException(ExceptionArgument.offset);
			}
			if (offset < 0)
			{
				return ThrowHelper.GetArgumentOutOfRangeException(ExceptionArgument.offset);
			}
			if (length < 0)
			{
				return ThrowHelper.GetArgumentOutOfRangeException(ExceptionArgument.length);
			}
			return ThrowHelper.GetArgumentException(ExceptionResource.Argument_InvalidOffsetLengthStringSegment);
		}
	}
}
