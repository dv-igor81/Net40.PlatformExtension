#define DEBUG
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Diagnostics.Tracing;

internal readonly struct PropertyValue
{
	[StructLayout(LayoutKind.Explicit)]
	public struct Scalar
	{
		[FieldOffset(0)]
		public bool AsBoolean;

		[FieldOffset(0)]
		public byte AsByte;

		[FieldOffset(0)]
		public sbyte AsSByte;

		[FieldOffset(0)]
		public char AsChar;

		[FieldOffset(0)]
		public short AsInt16;

		[FieldOffset(0)]
		public ushort AsUInt16;

		[FieldOffset(0)]
		public int AsInt32;

		[FieldOffset(0)]
		public uint AsUInt32;

		[FieldOffset(0)]
		public long AsInt64;

		[FieldOffset(0)]
		public ulong AsUInt64;

		[FieldOffset(0)]
		public IntPtr AsIntPtr;

		[FieldOffset(0)]
		public UIntPtr AsUIntPtr;

		[FieldOffset(0)]
		public float AsSingle;

		[FieldOffset(0)]
		public double AsDouble;

		[FieldOffset(0)]
		public Guid AsGuid;

		[FieldOffset(0)]
		public DateTime AsDateTime;

		[FieldOffset(0)]
		public DateTimeOffset AsDateTimeOffset;

		[FieldOffset(0)]
		public TimeSpan AsTimeSpan;

		[FieldOffset(0)]
		public decimal AsDecimal;
	}

	private abstract class TypeHelper
	{
		public abstract Func<PropertyValue, PropertyValue> GetPropertyGetter(PropertyInfo property);

		protected Delegate GetGetMethod(PropertyInfo property, Type propertyType)
		{
			return MethodInfoTheraotExtensions.CreateDelegate(property.GetMethod(), typeof(Func<, >).MakeGenericType(property.DeclaringType, propertyType));
		}
	}

	private sealed class ReferenceTypeHelper<TContainer> : TypeHelper where TContainer : class?
	{
		public override Func<PropertyValue, PropertyValue> GetPropertyGetter(PropertyInfo property)
		{
			PropertyInfo property2 = property;
			Type type = property2.PropertyType;
			if (!System.Diagnostics.Tracing.Statics.IsValueType(type))
			{
				Func<TContainer, object?> getter = (Func<TContainer, object>)GetGetMethod(property2, type);
				return (PropertyValue container) => new PropertyValue(getter((TContainer)container.ReferenceValue));
			}
			if (IntrospectionExtensions.GetTypeInfo(type).IsEnum)
			{
				type = Enum.GetUnderlyingType(type);
			}
			if (type == typeof(bool))
			{
				Func<TContainer, bool> f19 = (Func<TContainer, bool>)GetGetMethod(property2, type);
				return (PropertyValue container) => new PropertyValue(f19((TContainer)container.ReferenceValue));
			}
			if (type == typeof(byte))
			{
				Func<TContainer, byte> f18 = (Func<TContainer, byte>)GetGetMethod(property2, type);
				return (PropertyValue container) => new PropertyValue(f18((TContainer)container.ReferenceValue));
			}
			if (type == typeof(sbyte))
			{
				Func<TContainer, sbyte> f17 = (Func<TContainer, sbyte>)GetGetMethod(property2, type);
				return (PropertyValue container) => new PropertyValue(f17((TContainer)container.ReferenceValue));
			}
			if (type == typeof(char))
			{
				Func<TContainer, char> f16 = (Func<TContainer, char>)GetGetMethod(property2, type);
				return (PropertyValue container) => new PropertyValue(f16((TContainer)container.ReferenceValue));
			}
			if (type == typeof(short))
			{
				Func<TContainer, short> f15 = (Func<TContainer, short>)GetGetMethod(property2, type);
				return (PropertyValue container) => new PropertyValue(f15((TContainer)container.ReferenceValue));
			}
			if (type == typeof(ushort))
			{
				Func<TContainer, ushort> f14 = (Func<TContainer, ushort>)GetGetMethod(property2, type);
				return (PropertyValue container) => new PropertyValue(f14((TContainer)container.ReferenceValue));
			}
			if (type == typeof(int))
			{
				Func<TContainer, int> f13 = (Func<TContainer, int>)GetGetMethod(property2, type);
				return (PropertyValue container) => new PropertyValue(f13((TContainer)container.ReferenceValue));
			}
			if (type == typeof(uint))
			{
				Func<TContainer, uint> f12 = (Func<TContainer, uint>)GetGetMethod(property2, type);
				return (PropertyValue container) => new PropertyValue(f12((TContainer)container.ReferenceValue));
			}
			if (type == typeof(long))
			{
				Func<TContainer, long> f11 = (Func<TContainer, long>)GetGetMethod(property2, type);
				return (PropertyValue container) => new PropertyValue(f11((TContainer)container.ReferenceValue));
			}
			if (type == typeof(ulong))
			{
				Func<TContainer, ulong> f10 = (Func<TContainer, ulong>)GetGetMethod(property2, type);
				return (PropertyValue container) => new PropertyValue(f10((TContainer)container.ReferenceValue));
			}
			if (type == typeof(IntPtr))
			{
				Func<TContainer, IntPtr> f9 = (Func<TContainer, IntPtr>)GetGetMethod(property2, type);
				return (PropertyValue container) => new PropertyValue(f9((TContainer)container.ReferenceValue));
			}
			if (type == typeof(UIntPtr))
			{
				Func<TContainer, UIntPtr> f8 = (Func<TContainer, UIntPtr>)GetGetMethod(property2, type);
				return (PropertyValue container) => new PropertyValue(f8((TContainer)container.ReferenceValue));
			}
			if (type == typeof(float))
			{
				Func<TContainer, float> f7 = (Func<TContainer, float>)GetGetMethod(property2, type);
				return (PropertyValue container) => new PropertyValue(f7((TContainer)container.ReferenceValue));
			}
			if (type == typeof(double))
			{
				Func<TContainer, double> f6 = (Func<TContainer, double>)GetGetMethod(property2, type);
				return (PropertyValue container) => new PropertyValue(f6((TContainer)container.ReferenceValue));
			}
			if (type == typeof(Guid))
			{
				Func<TContainer, Guid> f5 = (Func<TContainer, Guid>)GetGetMethod(property2, type);
				return (PropertyValue container) => new PropertyValue(f5((TContainer)container.ReferenceValue));
			}
			if (type == typeof(DateTime))
			{
				Func<TContainer, DateTime> f4 = (Func<TContainer, DateTime>)GetGetMethod(property2, type);
				return (PropertyValue container) => new PropertyValue(f4((TContainer)container.ReferenceValue));
			}
			if (type == typeof(DateTimeOffset))
			{
				Func<TContainer, DateTimeOffset> f3 = (Func<TContainer, DateTimeOffset>)GetGetMethod(property2, type);
				return (PropertyValue container) => new PropertyValue(f3((TContainer)container.ReferenceValue));
			}
			if (type == typeof(TimeSpan))
			{
				Func<TContainer, TimeSpan> f2 = (Func<TContainer, TimeSpan>)GetGetMethod(property2, type);
				return (PropertyValue container) => new PropertyValue(f2((TContainer)container.ReferenceValue));
			}
			if (type == typeof(decimal))
			{
				Func<TContainer, decimal> f = (Func<TContainer, decimal>)GetGetMethod(property2, type);
				return (PropertyValue container) => new PropertyValue(f((TContainer)container.ReferenceValue));
			}
			return (PropertyValue container) => new PropertyValue(PropertyInfoTheraotExtensions.GetValue(property2, container.ReferenceValue));
		}
	}

	private readonly object? _reference;

	private readonly Scalar _scalar;

	private readonly int _scalarLength;

	public object? ReferenceValue
	{
		get
		{
			Debug.Assert(_scalarLength == 0, "This ReflectedValue refers to an unboxed value type, not a reference type or boxed value type.");
			return _reference;
		}
	}

	public Scalar ScalarValue
	{
		get
		{
			Debug.Assert(_scalarLength > 0, "This ReflectedValue refers to a reference type or boxed value type, not an unboxed value type");
			return _scalar;
		}
	}

	public int ScalarLength
	{
		get
		{
			Debug.Assert(_scalarLength > 0, "This ReflectedValue refers to a reference type or boxed value type, not an unboxed value type");
			return _scalarLength;
		}
	}

	private PropertyValue(object? value)
	{
		_reference = value;
		_scalar = default(Scalar);
		_scalarLength = 0;
	}

	private PropertyValue(Scalar scalar, int scalarLength)
	{
		_reference = null;
		_scalar = scalar;
		_scalarLength = scalarLength;
	}

	private PropertyValue(bool value)
		: this(new Scalar
		{
			AsBoolean = value
		}, 1)
	{
	}

	private PropertyValue(byte value)
		: this(new Scalar
		{
			AsByte = value
		}, 1)
	{
	}

	private PropertyValue(sbyte value)
		: this(new Scalar
		{
			AsSByte = value
		}, 1)
	{
	}

	private PropertyValue(char value)
		: this(new Scalar
		{
			AsChar = value
		}, 2)
	{
	}

	private PropertyValue(short value)
		: this(new Scalar
		{
			AsInt16 = value
		}, 2)
	{
	}

	private PropertyValue(ushort value)
		: this(new Scalar
		{
			AsUInt16 = value
		}, 2)
	{
	}

	private PropertyValue(int value)
		: this(new Scalar
		{
			AsInt32 = value
		}, 4)
	{
	}

	private PropertyValue(uint value)
		: this(new Scalar
		{
			AsUInt32 = value
		}, 4)
	{
	}

	private PropertyValue(long value)
		: this(new Scalar
		{
			AsInt64 = value
		}, 8)
	{
	}

	private PropertyValue(ulong value)
		: this(new Scalar
		{
			AsUInt64 = value
		}, 8)
	{
	}

	private unsafe PropertyValue(IntPtr value)
		: this(new Scalar
		{
			AsIntPtr = value
		}, sizeof(IntPtr))
	{
	}

	private unsafe PropertyValue(UIntPtr value)
		: this(new Scalar
		{
			AsUIntPtr = value
		}, sizeof(UIntPtr))
	{
	}

	private PropertyValue(float value)
		: this(new Scalar
		{
			AsSingle = value
		}, 4)
	{
	}

	private PropertyValue(double value)
		: this(new Scalar
		{
			AsDouble = value
		}, 8)
	{
	}

	private unsafe PropertyValue(Guid value)
		: this(new Scalar
		{
			AsGuid = value
		}, sizeof(Guid))
	{
	}

	private unsafe PropertyValue(DateTime value)
		: this(new Scalar
		{
			AsDateTime = value
		}, sizeof(DateTime))
	{
	}

	private unsafe PropertyValue(DateTimeOffset value)
		: this(new Scalar
		{
			AsDateTimeOffset = value
		}, sizeof(DateTimeOffset))
	{
	}

	private unsafe PropertyValue(TimeSpan value)
		: this(new Scalar
		{
			AsTimeSpan = value
		}, sizeof(TimeSpan))
	{
	}

	private PropertyValue(decimal value)
		: this(new Scalar
		{
			AsDecimal = value
		}, 16)
	{
	}

	public static Func<object?, PropertyValue> GetFactory(Type type)
	{
		if (type == typeof(bool))
		{
			return (object? value) => new PropertyValue((bool)value);
		}
		if (type == typeof(byte))
		{
			return (object? value) => new PropertyValue((byte)value);
		}
		if (type == typeof(sbyte))
		{
			return (object? value) => new PropertyValue((sbyte)value);
		}
		if (type == typeof(char))
		{
			return (object? value) => new PropertyValue((char)value);
		}
		if (type == typeof(short))
		{
			return (object? value) => new PropertyValue((short)value);
		}
		if (type == typeof(ushort))
		{
			return (object? value) => new PropertyValue((ushort)value);
		}
		if (type == typeof(int))
		{
			return (object? value) => new PropertyValue((int)value);
		}
		if (type == typeof(uint))
		{
			return (object? value) => new PropertyValue((uint)value);
		}
		if (type == typeof(long))
		{
			return (object? value) => new PropertyValue((long)value);
		}
		if (type == typeof(ulong))
		{
			return (object? value) => new PropertyValue((ulong)value);
		}
		if (type == typeof(IntPtr))
		{
			return (object? value) => new PropertyValue((IntPtr)value);
		}
		if (type == typeof(UIntPtr))
		{
			return (object? value) => new PropertyValue((UIntPtr)value);
		}
		if (type == typeof(float))
		{
			return (object? value) => new PropertyValue((float)value);
		}
		if (type == typeof(double))
		{
			return (object? value) => new PropertyValue((double)value);
		}
		if (type == typeof(Guid))
		{
			return (object? value) => new PropertyValue((Guid)value);
		}
		if (type == typeof(DateTime))
		{
			return (object? value) => new PropertyValue((DateTime)value);
		}
		if (type == typeof(DateTimeOffset))
		{
			return (object? value) => new PropertyValue((DateTimeOffset)value);
		}
		if (type == typeof(TimeSpan))
		{
			return (object? value) => new PropertyValue((TimeSpan)value);
		}
		if (type == typeof(decimal))
		{
			return (object? value) => new PropertyValue((decimal)value);
		}
		return (object? value) => new PropertyValue(value);
	}

	public static Func<PropertyValue, PropertyValue> GetPropertyGetter(PropertyInfo property)
	{
		if (IntrospectionExtensions.GetTypeInfo(property.DeclaringType).IsValueType)
		{
			return GetBoxedValueTypePropertyGetter(property);
		}
		return GetReferenceTypePropertyGetter(property);
	}

	private static Func<PropertyValue, PropertyValue> GetBoxedValueTypePropertyGetter(PropertyInfo property)
	{
		PropertyInfo property2 = property;
		Type type = property2.PropertyType;
		if (IntrospectionExtensions.GetTypeInfo(type).IsEnum)
		{
			type = Enum.GetUnderlyingType(type);
		}
		Func<object?, PropertyValue> factory = GetFactory(type);
		return (PropertyValue container) => factory(PropertyInfoTheraotExtensions.GetValue(property2, container.ReferenceValue));
	}

	private static Func<PropertyValue, PropertyValue> GetReferenceTypePropertyGetter(PropertyInfo property)
	{
		TypeHelper helper = (TypeHelper)Activator.CreateInstance(typeof(ReferenceTypeHelper<>).MakeGenericType(property.DeclaringType));
		return helper.GetPropertyGetter(property);
	}
}
