#define DEBUG
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Threading;

internal static class AsyncLocalValueMap
{
	private sealed class EmptyAsyncLocalValueMap : System.Threading.IAsyncLocalValueMap
	{
		public System.Threading.IAsyncLocalValueMap Set(System.Threading.IAsyncLocal key, object? value, bool treatNullValueAsNonexistent)
		{
			System.Threading.IAsyncLocalValueMap result;
			if (value == null && treatNullValueAsNonexistent)
			{
				System.Threading.IAsyncLocalValueMap asyncLocalValueMap = this;
				result = asyncLocalValueMap;
			}
			else
			{
				System.Threading.IAsyncLocalValueMap asyncLocalValueMap = new OneElementAsyncLocalValueMap(key, value);
				result = asyncLocalValueMap;
			}
			return result;
		}

		public bool TryGetValue(System.Threading.IAsyncLocal key, out object? value)
		{
			value = null;
			return false;
		}
	}

	private sealed class OneElementAsyncLocalValueMap : System.Threading.IAsyncLocalValueMap
	{
		private readonly System.Threading.IAsyncLocal _key1;

		private readonly object? _value1;

		public OneElementAsyncLocalValueMap(System.Threading.IAsyncLocal key, object? value)
		{
			_key1 = key;
			_value1 = value;
		}

		public System.Threading.IAsyncLocalValueMap Set(System.Threading.IAsyncLocal key, object? value, bool treatNullValueAsNonexistent)
		{
			if (value != null || !treatNullValueAsNonexistent)
			{
				System.Threading.IAsyncLocalValueMap result;
				if (key != _key1)
				{
					System.Threading.IAsyncLocalValueMap asyncLocalValueMap = new TwoElementAsyncLocalValueMap(_key1, _value1, key, value);
					result = asyncLocalValueMap;
				}
				else
				{
					System.Threading.IAsyncLocalValueMap asyncLocalValueMap = new OneElementAsyncLocalValueMap(key, value);
					result = asyncLocalValueMap;
				}
				return result;
			}
			System.Threading.IAsyncLocalValueMap result2;
			if (key != _key1)
			{
				System.Threading.IAsyncLocalValueMap asyncLocalValueMap = this;
				result2 = asyncLocalValueMap;
			}
			else
			{
				result2 = Empty;
			}
			return result2;
		}

		public bool TryGetValue(System.Threading.IAsyncLocal key, out object? value)
		{
			if (key == _key1)
			{
				value = _value1;
				return true;
			}
			value = null;
			return false;
		}
	}

	private sealed class TwoElementAsyncLocalValueMap : System.Threading.IAsyncLocalValueMap
	{
		private readonly System.Threading.IAsyncLocal _key1;

		private readonly System.Threading.IAsyncLocal _key2;

		private readonly object? _value1;

		private readonly object? _value2;

		public TwoElementAsyncLocalValueMap(System.Threading.IAsyncLocal key1, object? value1, System.Threading.IAsyncLocal key2, object? value2)
		{
			_key1 = key1;
			_value1 = value1;
			_key2 = key2;
			_value2 = value2;
		}

		public System.Threading.IAsyncLocalValueMap Set(System.Threading.IAsyncLocal key, object? value, bool treatNullValueAsNonexistent)
		{
			if (value != null || !treatNullValueAsNonexistent)
			{
				System.Threading.IAsyncLocalValueMap result;
				if (key != _key1)
				{
					if (key != _key2)
					{
						System.Threading.IAsyncLocalValueMap asyncLocalValueMap = new ThreeElementAsyncLocalValueMap(_key1, _value1, _key2, _value2, key, value);
						result = asyncLocalValueMap;
					}
					else
					{
						System.Threading.IAsyncLocalValueMap asyncLocalValueMap = new TwoElementAsyncLocalValueMap(_key1, _value1, key, value);
						result = asyncLocalValueMap;
					}
				}
				else
				{
					System.Threading.IAsyncLocalValueMap asyncLocalValueMap = new TwoElementAsyncLocalValueMap(key, value, _key2, _value2);
					result = asyncLocalValueMap;
				}
				return result;
			}
			System.Threading.IAsyncLocalValueMap result2;
			if (key != _key1)
			{
				if (key != _key2)
				{
					System.Threading.IAsyncLocalValueMap asyncLocalValueMap = this;
					result2 = asyncLocalValueMap;
				}
				else
				{
					System.Threading.IAsyncLocalValueMap asyncLocalValueMap = new OneElementAsyncLocalValueMap(_key1, _value1);
					result2 = asyncLocalValueMap;
				}
			}
			else
			{
				System.Threading.IAsyncLocalValueMap asyncLocalValueMap = new OneElementAsyncLocalValueMap(_key2, _value2);
				result2 = asyncLocalValueMap;
			}
			return result2;
		}

		public bool TryGetValue(System.Threading.IAsyncLocal key, out object? value)
		{
			if (key == _key1)
			{
				value = _value1;
				return true;
			}
			if (key == _key2)
			{
				value = _value2;
				return true;
			}
			value = null;
			return false;
		}
	}

	private sealed class ThreeElementAsyncLocalValueMap : System.Threading.IAsyncLocalValueMap
	{
		private readonly System.Threading.IAsyncLocal _key1;

		private readonly System.Threading.IAsyncLocal _key2;

		private readonly System.Threading.IAsyncLocal _key3;

		private readonly object? _value1;

		private readonly object? _value2;

		private readonly object? _value3;

		public ThreeElementAsyncLocalValueMap(System.Threading.IAsyncLocal key1, object? value1, System.Threading.IAsyncLocal key2, object? value2, System.Threading.IAsyncLocal key3, object? value3)
		{
			_key1 = key1;
			_value1 = value1;
			_key2 = key2;
			_value2 = value2;
			_key3 = key3;
			_value3 = value3;
		}

		public System.Threading.IAsyncLocalValueMap Set(System.Threading.IAsyncLocal key, object? value, bool treatNullValueAsNonexistent)
		{
			if (value != null || !treatNullValueAsNonexistent)
			{
				if (key == _key1)
				{
					return new ThreeElementAsyncLocalValueMap(key, value, _key2, _value2, _key3, _value3);
				}
				if (key == _key2)
				{
					return new ThreeElementAsyncLocalValueMap(_key1, _value1, key, value, _key3, _value3);
				}
				if (key == _key3)
				{
					return new ThreeElementAsyncLocalValueMap(_key1, _value1, _key2, _value2, key, value);
				}
				MultiElementAsyncLocalValueMap multi = new MultiElementAsyncLocalValueMap(4);
				multi.UnsafeStore(0, _key1, _value1);
				multi.UnsafeStore(1, _key2, _value2);
				multi.UnsafeStore(2, _key3, _value3);
				multi.UnsafeStore(3, key, value);
				return multi;
			}
			System.Threading.IAsyncLocalValueMap result;
			if (key != _key1)
			{
				if (key != _key2)
				{
					if (key != _key3)
					{
						System.Threading.IAsyncLocalValueMap asyncLocalValueMap = this;
						result = asyncLocalValueMap;
					}
					else
					{
						System.Threading.IAsyncLocalValueMap asyncLocalValueMap = new TwoElementAsyncLocalValueMap(_key1, _value1, _key2, _value2);
						result = asyncLocalValueMap;
					}
				}
				else
				{
					System.Threading.IAsyncLocalValueMap asyncLocalValueMap = new TwoElementAsyncLocalValueMap(_key1, _value1, _key3, _value3);
					result = asyncLocalValueMap;
				}
			}
			else
			{
				System.Threading.IAsyncLocalValueMap asyncLocalValueMap = new TwoElementAsyncLocalValueMap(_key2, _value2, _key3, _value3);
				result = asyncLocalValueMap;
			}
			return result;
		}

		public bool TryGetValue(System.Threading.IAsyncLocal key, out object? value)
		{
			if (key == _key1)
			{
				value = _value1;
				return true;
			}
			if (key == _key2)
			{
				value = _value2;
				return true;
			}
			if (key == _key3)
			{
				value = _value3;
				return true;
			}
			value = null;
			return false;
		}
	}

	private sealed class MultiElementAsyncLocalValueMap : System.Threading.IAsyncLocalValueMap
	{
		internal const int MaxMultiElements = 16;

		private readonly KeyValuePair<System.Threading.IAsyncLocal, object?>[] _keyValues;

		internal MultiElementAsyncLocalValueMap(int count)
		{
			Debug.Assert(count <= 16);
			_keyValues = new KeyValuePair<System.Threading.IAsyncLocal, object>[count];
		}

		internal void UnsafeStore(int index, System.Threading.IAsyncLocal key, object? value)
		{
			Debug.Assert(index < _keyValues.Length);
			_keyValues[index] = new KeyValuePair<System.Threading.IAsyncLocal, object>(key, value);
		}

		public System.Threading.IAsyncLocalValueMap Set(System.Threading.IAsyncLocal key, object? value, bool treatNullValueAsNonexistent)
		{
			for (int i = 0; i < _keyValues.Length; i++)
			{
				if (key != _keyValues[i].Key)
				{
					continue;
				}
				if (value != null || !treatNullValueAsNonexistent)
				{
					MultiElementAsyncLocalValueMap multi3 = new MultiElementAsyncLocalValueMap(_keyValues.Length);
					Array.Copy(_keyValues, 0, multi3._keyValues, 0, _keyValues.Length);
					multi3._keyValues[i] = new KeyValuePair<System.Threading.IAsyncLocal, object>(key, value);
					return multi3;
				}
				if (_keyValues.Length == 4)
				{
					System.Threading.IAsyncLocalValueMap result;
					switch (i)
					{
					default:
					{
						System.Threading.IAsyncLocalValueMap asyncLocalValueMap = new ThreeElementAsyncLocalValueMap(_keyValues[0].Key, _keyValues[0].Value, _keyValues[1].Key, _keyValues[1].Value, _keyValues[2].Key, _keyValues[2].Value);
						result = asyncLocalValueMap;
						break;
					}
					case 2:
					{
						System.Threading.IAsyncLocalValueMap asyncLocalValueMap = new ThreeElementAsyncLocalValueMap(_keyValues[0].Key, _keyValues[0].Value, _keyValues[1].Key, _keyValues[1].Value, _keyValues[3].Key, _keyValues[3].Value);
						result = asyncLocalValueMap;
						break;
					}
					case 1:
					{
						System.Threading.IAsyncLocalValueMap asyncLocalValueMap = new ThreeElementAsyncLocalValueMap(_keyValues[0].Key, _keyValues[0].Value, _keyValues[2].Key, _keyValues[2].Value, _keyValues[3].Key, _keyValues[3].Value);
						result = asyncLocalValueMap;
						break;
					}
					case 0:
					{
						System.Threading.IAsyncLocalValueMap asyncLocalValueMap = new ThreeElementAsyncLocalValueMap(_keyValues[1].Key, _keyValues[1].Value, _keyValues[2].Key, _keyValues[2].Value, _keyValues[3].Key, _keyValues[3].Value);
						result = asyncLocalValueMap;
						break;
					}
					}
					return result;
				}
				MultiElementAsyncLocalValueMap multi = new MultiElementAsyncLocalValueMap(_keyValues.Length - 1);
				if (i != 0)
				{
					Array.Copy(_keyValues, 0, multi._keyValues, 0, i);
				}
				if (i != _keyValues.Length - 1)
				{
					Array.Copy(_keyValues, i + 1, multi._keyValues, i, _keyValues.Length - i - 1);
				}
				return multi;
			}
			if (value == null && treatNullValueAsNonexistent)
			{
				return this;
			}
			if (_keyValues.Length < 16)
			{
				MultiElementAsyncLocalValueMap multi2 = new MultiElementAsyncLocalValueMap(_keyValues.Length + 1);
				Array.Copy(_keyValues, 0, multi2._keyValues, 0, _keyValues.Length);
				multi2._keyValues[_keyValues.Length] = new KeyValuePair<System.Threading.IAsyncLocal, object>(key, value);
				return multi2;
			}
			ManyElementAsyncLocalValueMap many = new ManyElementAsyncLocalValueMap(17);
			KeyValuePair<System.Threading.IAsyncLocal, object>[] keyValues = _keyValues;
			for (int j = 0; j < keyValues.Length; j++)
			{
				KeyValuePair<System.Threading.IAsyncLocal, object> pair = keyValues[j];
				many[pair.Key] = pair.Value;
			}
			many[key] = value;
			return many;
		}

		public bool TryGetValue(System.Threading.IAsyncLocal key, out object? value)
		{
			KeyValuePair<System.Threading.IAsyncLocal, object>[] keyValues = _keyValues;
			for (int i = 0; i < keyValues.Length; i++)
			{
				KeyValuePair<System.Threading.IAsyncLocal, object> pair = keyValues[i];
				if (key == pair.Key)
				{
					value = pair.Value;
					return true;
				}
			}
			value = null;
			return false;
		}
	}

	private sealed class ManyElementAsyncLocalValueMap : Dictionary<System.Threading.IAsyncLocal, object?>, System.Threading.IAsyncLocalValueMap
	{
		public ManyElementAsyncLocalValueMap(int capacity)
			: base(capacity)
		{
		}

		public System.Threading.IAsyncLocalValueMap Set(System.Threading.IAsyncLocal key, object? value, bool treatNullValueAsNonexistent)
		{
			int count = base.Count;
			bool containsKey = ContainsKey(key);
			if (value != null || !treatNullValueAsNonexistent)
			{
				ManyElementAsyncLocalValueMap map = new ManyElementAsyncLocalValueMap(count + ((!containsKey) ? 1 : 0));
				using (Dictionary<System.Threading.IAsyncLocal, object>.Enumerator enumerator = GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<System.Threading.IAsyncLocal, object> pair = enumerator.Current;
						map[pair.Key] = pair.Value;
					}
				}
				map[key] = value;
				return map;
			}
			if (containsKey)
			{
				if (count == 17)
				{
					MultiElementAsyncLocalValueMap multi = new MultiElementAsyncLocalValueMap(16);
					int index = 0;
					using (Dictionary<System.Threading.IAsyncLocal, object>.Enumerator enumerator2 = GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							KeyValuePair<System.Threading.IAsyncLocal, object> pair2 = enumerator2.Current;
							if (key != pair2.Key)
							{
								multi.UnsafeStore(index++, pair2.Key, pair2.Value);
							}
						}
					}
					Debug.Assert(index == 16);
					return multi;
				}
				ManyElementAsyncLocalValueMap map2 = new ManyElementAsyncLocalValueMap(count - 1);
				using (Dictionary<System.Threading.IAsyncLocal, object>.Enumerator enumerator3 = GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						KeyValuePair<System.Threading.IAsyncLocal, object> pair3 = enumerator3.Current;
						if (key != pair3.Key)
						{
							map2[pair3.Key] = pair3.Value;
						}
					}
				}
				Debug.Assert(map2.Count == count - 1);
				return map2;
			}
			return this;
		}
	}

	public static System.Threading.IAsyncLocalValueMap Empty { get; } = new EmptyAsyncLocalValueMap();


	public static bool IsEmpty(System.Threading.IAsyncLocalValueMap asyncLocalValueMap)
	{
		Debug.Assert(asyncLocalValueMap != null);
		Debug.Assert(asyncLocalValueMap == Empty || asyncLocalValueMap.GetType() != typeof(EmptyAsyncLocalValueMap));
		return asyncLocalValueMap == Empty;
	}

	public static System.Threading.IAsyncLocalValueMap Create(System.Threading.IAsyncLocal key, object? value, bool treatNullValueAsNonexistent)
	{
		System.Threading.IAsyncLocalValueMap result;
		if (value == null && treatNullValueAsNonexistent)
		{
			result = Empty;
		}
		else
		{
			System.Threading.IAsyncLocalValueMap asyncLocalValueMap = new OneElementAsyncLocalValueMap(key, value);
			result = asyncLocalValueMap;
		}
		return result;
	}
}
