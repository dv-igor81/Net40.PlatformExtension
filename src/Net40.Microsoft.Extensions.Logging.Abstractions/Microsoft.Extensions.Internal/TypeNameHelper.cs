using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.Internal;

internal class TypeNameHelper
{
	private readonly struct DisplayNameOptions
	{
		public bool FullName { get; }

		public bool IncludeGenericParameters { get; }

		public bool IncludeGenericParameterNames { get; }

		public char NestedTypeDelimiter { get; }

		public DisplayNameOptions(bool fullName, bool includeGenericParameterNames, bool includeGenericParameters, char nestedTypeDelimiter)
		{
			FullName = fullName;
			IncludeGenericParameters = includeGenericParameters;
			IncludeGenericParameterNames = includeGenericParameterNames;
			NestedTypeDelimiter = nestedTypeDelimiter;
		}
	}

	private const char DefaultNestedTypeDelimiter = '+';

	private static readonly Dictionary<Type, string> _builtInTypeNames = new Dictionary<Type, string>
	{
		{
			typeof(void),
			"void"
		},
		{
			typeof(bool),
			"bool"
		},
		{
			typeof(byte),
			"byte"
		},
		{
			typeof(char),
			"char"
		},
		{
			typeof(decimal),
			"decimal"
		},
		{
			typeof(double),
			"double"
		},
		{
			typeof(float),
			"float"
		},
		{
			typeof(int),
			"int"
		},
		{
			typeof(long),
			"long"
		},
		{
			typeof(object),
			"object"
		},
		{
			typeof(sbyte),
			"sbyte"
		},
		{
			typeof(short),
			"short"
		},
		{
			typeof(string),
			"string"
		},
		{
			typeof(uint),
			"uint"
		},
		{
			typeof(ulong),
			"ulong"
		},
		{
			typeof(ushort),
			"ushort"
		}
	};

	public static string GetTypeDisplayName(object item, bool fullName = true)
	{
		if (item != null)
		{
			return GetTypeDisplayName(item.GetType(), fullName);
		}
		return null;
	}

	public static string GetTypeDisplayName(Type type, bool fullName = true, bool includeGenericParameterNames = false, bool includeGenericParameters = true, char nestedTypeDelimiter = '+')
	{
		StringBuilder stringBuilder = new StringBuilder();
		DisplayNameOptions options = new DisplayNameOptions(fullName, includeGenericParameterNames, includeGenericParameters, nestedTypeDelimiter);
		ProcessType(stringBuilder, type, in options);
		return stringBuilder.ToString();
	}

	private static void ProcessType(StringBuilder builder, Type type, in DisplayNameOptions options)
	{
		if (type.IsGenericType)
		{
			Type[] genericArguments = type.GetGenericArguments();
			ProcessGenericType(builder, type, genericArguments, genericArguments.Length, in options);
			return;
		}
		if (type.IsArray)
		{
			ProcessArrayType(builder, type, in options);
			return;
		}
		if (_builtInTypeNames.TryGetValue(type, out string value))
		{
			builder.Append(value);
			return;
		}
		if (type.IsGenericParameter)
		{
			if (options.IncludeGenericParameterNames)
			{
				builder.Append(type.Name);
			}
			return;
		}
		string text = (options.FullName ? type.FullName : type.Name);
		builder.Append(text);
		if (options.NestedTypeDelimiter != '+')
		{
			builder.Replace('+', options.NestedTypeDelimiter, builder.Length - text.Length, text.Length);
		}
	}

	private static void ProcessArrayType(StringBuilder builder, Type type, in DisplayNameOptions options)
	{
		Type type2 = type;
		while (type2.IsArray)
		{
			type2 = type2.GetElementType();
		}
		ProcessType(builder, type2, in options);
		while (type.IsArray)
		{
			builder.Append('[');
			builder.Append(',', type.GetArrayRank() - 1);
			builder.Append(']');
			type = type.GetElementType();
		}
	}

	private static void ProcessGenericType(StringBuilder builder, Type type, Type[] genericArguments, int length, in DisplayNameOptions options)
	{
		int num = 0;
		if (type.IsNested)
		{
			num = type.DeclaringType.GetGenericArguments().Length;
		}
		if (options.FullName)
		{
			if (type.IsNested)
			{
				ProcessGenericType(builder, type.DeclaringType, genericArguments, num, in options);
				builder.Append(options.NestedTypeDelimiter);
			}
			else if (!string.IsNullOrEmpty(type.Namespace))
			{
				builder.Append(type.Namespace);
				builder.Append('.');
			}
		}
		int num2 = type.Name.IndexOf('`');
		if (num2 <= 0)
		{
			builder.Append(type.Name);
			return;
		}
		builder.Append(type.Name, 0, num2);
		if (!options.IncludeGenericParameters)
		{
			return;
		}
		builder.Append('<');
		for (int i = num; i < length; i++)
		{
			ProcessType(builder, genericArguments[i], in options);
			if (i + 1 != length)
			{
				builder.Append(',');
				if (options.IncludeGenericParameterNames || !genericArguments[i + 1].IsGenericParameter)
				{
					builder.Append(' ');
				}
			}
		}
		builder.Append('>');
	}
}
