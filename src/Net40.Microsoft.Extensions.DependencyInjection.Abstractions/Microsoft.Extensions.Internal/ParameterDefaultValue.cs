using System;
using System.Reflection;

namespace Microsoft.Extensions.Internal;

public class ParameterDefaultValue
{
	private static readonly Type _nullable = typeof(Nullable<>);

	public static bool TryGetDefaultValue(ParameterInfo parameter, out object defaultValue)
	{
		bool flag = true;
		defaultValue = null;
		bool flag2;
		try
		{
			flag2 = parameter.HasDefaultValue();
		}
		catch (FormatException) when (parameter.ParameterType == typeof(DateTime))
		{
			flag2 = true;
			flag = false;
		}
		if (flag2)
		{
			if (flag)
			{
				defaultValue = parameter.DefaultValue;
			}
			if (defaultValue == null && parameter.ParameterType.IsValueType)
			{
				defaultValue = Activator.CreateInstance(parameter.ParameterType);
			}
			if (defaultValue != null && parameter.ParameterType.IsGenericType && parameter.ParameterType.GetGenericTypeDefinition() == _nullable)
			{
				Type underlyingType = Nullable.GetUnderlyingType(parameter.ParameterType);
				if (underlyingType != null && underlyingType.IsEnum)
				{
					defaultValue = Enum.ToObject(underlyingType, defaultValue);
				}
			}
		}
		return flag2;
	}
}
