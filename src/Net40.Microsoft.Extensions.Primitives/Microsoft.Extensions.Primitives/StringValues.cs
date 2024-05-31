using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Internal;

namespace Microsoft.Extensions.Primitives;

public readonly struct StringValues : IList<string>, ICollection<string>, IEnumerable<string>, IEnumerable, IReadOnlyList<string>, IReadOnlyCollection<string>, IEquatable<StringValues>, IEquatable<string>, IEquatable<string[]>
{
	public struct Enumerator : IEnumerator<string>, IDisposable, IEnumerator
	{
		private readonly string[] _values;

		private string _current;

		private int _index;

		public string Current => _current;

		object IEnumerator.Current => _current;

		internal Enumerator(object value)
		{
			if (value is string current)
			{
				_values = null;
				_current = current;
			}
			else
			{
				_current = null;
				_values = Unsafe.As<string[]>(value);
			}
			_index = 0;
		}

		public Enumerator(ref StringValues values)
			: this(values._values)
		{
		}

		public bool MoveNext()
		{
			int index = _index;
			if (index < 0)
			{
				return false;
			}
			string[] values = _values;
			if (values != null)
			{
				if ((uint)index < (uint)values.Length)
				{
					_index = index + 1;
					_current = values[index];
					return true;
				}
				_index = -1;
				return false;
			}
			_index = -1;
			return _current != null;
		}

		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		public void Dispose()
		{
		}
	}

	public static readonly StringValues Empty = new StringValues(ArrayEx.Empty<string>());

	private readonly object _values;

	public int Count
	{
		[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
		get
		{
			object values = _values;
			if (values is string)
			{
				return 1;
			}
			if (values == null)
			{
				return 0;
			}
			return Unsafe.As<string[]>(values).Length;
		}
	}

	bool ICollection<string>.IsReadOnly => true;

	string IList<string>.this[int index]
	{
		get
		{
			return this[index];
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public string this[int index]
	{
		[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
		get
		{
			object values = _values;
			if (index == 0 && values is string result)
			{
				return result;
			}
			if (values != null)
			{
				return Unsafe.As<string[]>(values)[index];
			}
			return OutOfBounds();
		}
	}

	public StringValues(string value)
	{
		_values = value;
	}

	public StringValues(string[] values)
	{
		_values = values;
	}

	public static implicit operator StringValues(string value)
	{
		return new StringValues(value);
	}

	public static implicit operator StringValues(string[] values)
	{
		return new StringValues(values);
	}

	public static implicit operator string(StringValues values)
	{
		return values.GetStringValue();
	}

	public static implicit operator string[](StringValues value)
	{
		return value.GetArrayValue();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static string OutOfBounds()
	{
		return ArrayEx.Empty<string>()[0];
	}

	public override string ToString()
	{
		return GetStringValue() ?? string.Empty;
	}

	private string GetStringValue()
	{
		object values2 = _values;
		if (values2 is string result)
		{
			return result;
		}
		return GetStringValueFromArray(values2);
		static string GetJoinedStringValueFromArray(string[] values)
		{
			int num = 0;
			foreach (string text in values)
			{
				if (text != null && text.Length > 0)
				{
					if (num > 0)
					{
						num++;
					}
					num += text.Length;
				}
			}
			InplaceStringBuilder inplaceStringBuilder = new InplaceStringBuilder(num);
			bool flag = false;
			foreach (string text2 in values)
			{
				if (text2 != null && text2.Length > 0)
				{
					if (flag)
					{
						inplaceStringBuilder.Append(',');
					}
					inplaceStringBuilder.Append(text2);
					flag = true;
				}
			}
			return inplaceStringBuilder.ToString();
		}
		static string GetStringValueFromArray(object value)
		{
			if (value == null)
			{
				return null;
			}
			string[] array = Unsafe.As<string[]>(value);
			return array.Length switch
			{
				0 => null, 
				1 => array[0], 
				_ => GetJoinedStringValueFromArray(array), 
			};
		}
	}

	public string[] ToArray()
	{
		return GetArrayValue() ?? ArrayEx.Empty<string>();
	}

	private string[] GetArrayValue()
	{
		object values = _values;
		if (values is string[] result)
		{
			return result;
		}
		if (values != null)
		{
			return new string[1] { Unsafe.As<string>(values) };
		}
		return null;
	}

	int IList<string>.IndexOf(string item)
	{
		return IndexOf(item);
	}

	private int IndexOf(string item)
	{
		object values = _values;
		if (values is string[] array)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (string.Equals(array[i], item, StringComparison.Ordinal))
				{
					return i;
				}
			}
			return -1;
		}
		if (values != null)
		{
			if (!string.Equals(Unsafe.As<string>(values), item, StringComparison.Ordinal))
			{
				return -1;
			}
			return 0;
		}
		return -1;
	}

	bool ICollection<string>.Contains(string item)
	{
		return IndexOf(item) >= 0;
	}

	void ICollection<string>.CopyTo(string[] array, int arrayIndex)
	{
		CopyTo(array, arrayIndex);
	}

	private void CopyTo(string[] array, int arrayIndex)
	{
		object values = _values;
		if (values is string[] array2)
		{
			Array.Copy(array2, 0, array, arrayIndex, array2.Length);
		}
		else if (values != null)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("arrayIndex");
			}
			if (array.Length - arrayIndex < 1)
			{
				throw new ArgumentException("'array' is not long enough to copy all the items in the collection. Check 'arrayIndex' and 'array' length.");
			}
			array[arrayIndex] = Unsafe.As<string>(values);
		}
	}

	void ICollection<string>.Add(string item)
	{
		throw new NotSupportedException();
	}

	void IList<string>.Insert(int index, string item)
	{
		throw new NotSupportedException();
	}

	bool ICollection<string>.Remove(string item)
	{
		throw new NotSupportedException();
	}

	void IList<string>.RemoveAt(int index)
	{
		throw new NotSupportedException();
	}

	void ICollection<string>.Clear()
	{
		throw new NotSupportedException();
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(_values);
	}

	IEnumerator<string> IEnumerable<string>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public static bool IsNullOrEmpty(StringValues value)
	{
		object values = value._values;
		if (values == null)
		{
			return true;
		}
		if (values is string[] array)
		{
			return array.Length switch
			{
				0 => true, 
				1 => string.IsNullOrEmpty(array[0]), 
				_ => false, 
			};
		}
		return string.IsNullOrEmpty(Unsafe.As<string>(values));
	}

	public static StringValues Concat(StringValues values1, StringValues values2)
	{
		int count = values1.Count;
		int count2 = values2.Count;
		if (count == 0)
		{
			return values2;
		}
		if (count2 == 0)
		{
			return values1;
		}
		string[] array = new string[count + count2];
		values1.CopyTo(array, 0);
		values2.CopyTo(array, count);
		return new StringValues(array);
	}

	public static StringValues Concat(in StringValues values, string value)
	{
		if (value == null)
		{
			return values;
		}
		int count = values.Count;
		if (count == 0)
		{
			return new StringValues(value);
		}
		string[] array = new string[count + 1];
		values.CopyTo(array, 0);
		array[count] = value;
		return new StringValues(array);
	}

	public static StringValues Concat(string value, in StringValues values)
	{
		if (value == null)
		{
			return values;
		}
		int count = values.Count;
		if (count == 0)
		{
			return new StringValues(value);
		}
		string[] array = new string[count + 1];
		array[0] = value;
		values.CopyTo(array, 1);
		return new StringValues(array);
	}

	public static bool Equals(StringValues left, StringValues right)
	{
		int count = left.Count;
		if (count != right.Count)
		{
			return false;
		}
		for (int i = 0; i < count; i++)
		{
			if (left[i] != right[i])
			{
				return false;
			}
		}
		return true;
	}

	public static bool operator ==(StringValues left, StringValues right)
	{
		return Equals(left, right);
	}

	public static bool operator !=(StringValues left, StringValues right)
	{
		return !Equals(left, right);
	}

	public bool Equals(StringValues other)
	{
		return Equals(this, other);
	}

	public static bool Equals(string left, StringValues right)
	{
		return Equals(new StringValues(left), right);
	}

	public static bool Equals(StringValues left, string right)
	{
		return Equals(left, new StringValues(right));
	}

	public bool Equals(string other)
	{
		return Equals(this, new StringValues(other));
	}

	public static bool Equals(string[] left, StringValues right)
	{
		return Equals(new StringValues(left), right);
	}

	public static bool Equals(StringValues left, string[] right)
	{
		return Equals(left, new StringValues(right));
	}

	public bool Equals(string[] other)
	{
		return Equals(this, new StringValues(other));
	}

	public static bool operator ==(StringValues left, string right)
	{
		return Equals(left, new StringValues(right));
	}

	public static bool operator !=(StringValues left, string right)
	{
		return !Equals(left, new StringValues(right));
	}

	public static bool operator ==(string left, StringValues right)
	{
		return Equals(new StringValues(left), right);
	}

	public static bool operator !=(string left, StringValues right)
	{
		return !Equals(new StringValues(left), right);
	}

	public static bool operator ==(StringValues left, string[] right)
	{
		return Equals(left, new StringValues(right));
	}

	public static bool operator !=(StringValues left, string[] right)
	{
		return !Equals(left, new StringValues(right));
	}

	public static bool operator ==(string[] left, StringValues right)
	{
		return Equals(new StringValues(left), right);
	}

	public static bool operator !=(string[] left, StringValues right)
	{
		return !Equals(new StringValues(left), right);
	}

	public static bool operator ==(StringValues left, object right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(StringValues left, object right)
	{
		return !left.Equals(right);
	}

	public static bool operator ==(object left, StringValues right)
	{
		return right.Equals(left);
	}

	public static bool operator !=(object left, StringValues right)
	{
		return !right.Equals(left);
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return Equals(this, Empty);
		}
		if (obj is string)
		{
			return Equals(this, (string)obj);
		}
		if (obj is string[])
		{
			return Equals(this, (string[])obj);
		}
		if (obj is StringValues)
		{
			return Equals(this, (StringValues)obj);
		}
		return false;
	}

	public override int GetHashCode()
	{
		object values = _values;
		if (values is string[] array)
		{
			if (Count == 1)
			{
				return Unsafe.As<string>(this[0])?.GetHashCode() ?? Count.GetHashCode();
			}
			HashCodeCombiner hashCodeCombiner = default(HashCodeCombiner);
			for (int i = 0; i < array.Length; i++)
			{
				hashCodeCombiner.Add(array[i]);
			}
			return hashCodeCombiner.CombinedHash;
		}
		return Unsafe.As<string>(values)?.GetHashCode() ?? Count.GetHashCode();
	}
}
