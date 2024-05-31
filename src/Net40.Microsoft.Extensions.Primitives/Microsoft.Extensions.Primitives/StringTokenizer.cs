using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Extensions.Primitives;

public readonly struct StringTokenizer : IEnumerable<StringSegment>, IEnumerable
{
	public struct Enumerator : IEnumerator<StringSegment>, IDisposable, IEnumerator
	{
		private readonly StringSegment _value;

		private readonly char[] _separators;

		private int _index;

		public StringSegment Current { get; private set; }

		object IEnumerator.Current => Current;

		internal Enumerator(in StringSegment value, char[] separators)
		{
			_value = value;
			_separators = separators;
			Current = default(StringSegment);
			_index = 0;
		}

		public Enumerator(ref StringTokenizer tokenizer)
		{
			_value = tokenizer._value;
			_separators = tokenizer._separators;
			Current = default(StringSegment);
			_index = 0;
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			if (!_value.HasValue || _index > _value.Length)
			{
				Current = default(StringSegment);
				return false;
			}
			int num = _value.IndexOfAny(_separators, _index);
			if (num == -1)
			{
				num = _value.Length;
			}
			Current = _value.Subsegment(_index, num - _index);
			_index = num + 1;
			return true;
		}

		public void Reset()
		{
			Current = default(StringSegment);
			_index = 0;
		}
	}

	private readonly StringSegment _value;

	private readonly char[] _separators;

	public StringTokenizer(string value, char[] separators)
	{
		if (value == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
		}
		if (separators == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.separators);
		}
		_value = value;
		_separators = separators;
	}

	public StringTokenizer(StringSegment value, char[] separators)
	{
		if (!value.HasValue)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
		}
		if (separators == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.separators);
		}
		_value = value;
		_separators = separators;
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(in _value, _separators);
	}

	IEnumerator<StringSegment> IEnumerable<StringSegment>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
