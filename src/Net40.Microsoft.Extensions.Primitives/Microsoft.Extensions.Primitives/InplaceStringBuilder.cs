using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.Extensions.Primitives;

[DebuggerDisplay("Value = {_value}")]
[Obsolete("This type is obsolete and will be removed in a future version.")]
public struct InplaceStringBuilder
{
	private int _offset;

	private int _capacity;

	private string _value;

	public int Capacity
	{
		get
		{
			return _capacity;
		}
		set
		{
			if (value < 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.value);
			}
			if (_offset > 0)
			{
				ThrowHelper.ThrowInvalidOperationException(ExceptionResource.Capacity_CannotChangeAfterWriteStarted);
			}
			_capacity = value;
		}
	}

	public InplaceStringBuilder(int capacity)
	{
		this = default(InplaceStringBuilder);
		if (capacity < 0)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.capacity);
		}
		_capacity = capacity;
	}

	public void Append(string value)
	{
		if (value == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
		}
		Append(value, 0, value.Length);
	}

	public void Append(StringSegment segment)
	{
		Append(segment.Buffer, segment.Offset, segment.Length);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public unsafe void Append(string value, int offset, int count)
	{
		EnsureValueIsInitialized();
		if (value == null || offset < 0 || value.Length - offset < count || Capacity - _offset < count)
		{
			ThrowValidationError(value, offset, count);
		}
		fixed (char* ptr = _value)
		{
			fixed (char* ptr2 = value)
			{
				Unsafe.CopyBlockUnaligned(ptr + _offset, ptr2 + offset, (uint)(count * 2));
				_offset += count;
			}
		}
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public unsafe void Append(char c)
	{
		EnsureValueIsInitialized();
		if (_offset >= Capacity)
		{
			ThrowHelper.ThrowInvalidOperationException(ExceptionResource.Capacity_NotEnough, 1, Capacity - _offset);
		}
		fixed (char* ptr = _value)
		{
			ptr[_offset++] = c;
		}
	}

	public override string ToString()
	{
		if (Capacity != _offset)
		{
			ThrowHelper.ThrowInvalidOperationException(ExceptionResource.Capacity_NotUsedEntirely, Capacity, _offset);
		}
		return _value;
	}

	private void EnsureValueIsInitialized()
	{
		if (_value == null)
		{
			_value = new string('\0', _capacity);
		}
	}

	private void ThrowValidationError(string value, int offset, int count)
	{
		if (value == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
		}
		if (offset < 0 || value.Length - offset < count)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.offset);
		}
		if (Capacity - _offset < count)
		{
			ThrowHelper.ThrowInvalidOperationException(ExceptionResource.Capacity_NotEnough, value.Length, Capacity - _offset);
		}
	}
}
