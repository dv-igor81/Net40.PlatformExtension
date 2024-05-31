using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.Extensions.Internal;

internal struct HashCodeCombiner
{
	private long _combinedHash64;

	public int CombinedHash
	{
		[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
		get
		{
			return _combinedHash64.GetHashCode();
		}
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	private HashCodeCombiner(long seed)
	{
		_combinedHash64 = seed;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public void Add(IEnumerable e)
	{
		if (e == null)
		{
			Add(0);
			return;
		}
		int count = 0;
		foreach (object o in e)
		{
			Add(o);
			count++;
		}
		Add(count);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static implicit operator int(HashCodeCombiner self)
	{
		return self.CombinedHash;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public void Add(int i)
	{
		_combinedHash64 = ((_combinedHash64 << 5) + _combinedHash64) ^ i;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public void Add(string s)
	{
		int hashCode = s?.GetHashCode() ?? 0;
		Add(hashCode);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public void Add(object o)
	{
		int hashCode = o?.GetHashCode() ?? 0;
		Add(hashCode);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public void Add<TValue>(TValue value, IEqualityComparer<TValue> comparer)
	{
		int hashCode = ((value != null) ? comparer.GetHashCode(value) : 0);
		Add(hashCode);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static HashCodeCombiner Start()
	{
		return new HashCodeCombiner(5381L);
	}
}
