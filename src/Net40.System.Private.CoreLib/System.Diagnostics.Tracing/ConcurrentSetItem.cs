namespace System.Diagnostics.Tracing;

internal abstract class ConcurrentSetItem<KeyType, ItemType> where ItemType : System.Diagnostics.Tracing.ConcurrentSetItem<KeyType, ItemType>
{
	public abstract int Compare(ItemType other);

	public abstract int Compare(KeyType key);
}
