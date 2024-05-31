using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microsoft.Extensions.Logging;

public static class LoggerMessage
{
	private readonly struct LogValues : IReadOnlyList<KeyValuePair<string, object>>, IReadOnlyCollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable
	{
		public static readonly Func<LogValues, Exception, string> Callback = (LogValues state, Exception exception) => state.ToString();

		private readonly LogValuesFormatter _formatter;

		public KeyValuePair<string, object> this[int index]
		{
			get
			{
				if (index == 0)
				{
					return new KeyValuePair<string, object>("{OriginalFormat}", _formatter.OriginalFormat);
				}
				throw new IndexOutOfRangeException("index");
			}
		}

		public int Count => 1;

		public LogValues(LogValuesFormatter formatter)
		{
			_formatter = formatter;
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			yield return this[0];
		}

		public override string ToString()
		{
			return _formatter.Format();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	private readonly struct LogValues<T0> : IReadOnlyList<KeyValuePair<string, object>>, IReadOnlyCollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable
	{
		public static readonly Func<LogValues<T0>, Exception, string> Callback = (LogValues<T0> state, Exception exception) => state.ToString();

		private readonly LogValuesFormatter _formatter;

		private readonly T0 _value0;

		public KeyValuePair<string, object> this[int index] => index switch
		{
			0 => new KeyValuePair<string, object>(_formatter.ValueNames[0], _value0), 
			1 => new KeyValuePair<string, object>("{OriginalFormat}", _formatter.OriginalFormat), 
			_ => throw new IndexOutOfRangeException("index"), 
		};

		public int Count => 2;

		public LogValues(LogValuesFormatter formatter, T0 value0)
		{
			_formatter = formatter;
			_value0 = value0;
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			int i = 0;
			while (i < Count)
			{
				yield return this[i];
				int num = i + 1;
				i = num;
			}
		}

		public override string ToString()
		{
			return _formatter.Format(_value0);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	private readonly struct LogValues<T0, T1> : IReadOnlyList<KeyValuePair<string, object>>, IReadOnlyCollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable
	{
		public static readonly Func<LogValues<T0, T1>, Exception, string> Callback = (LogValues<T0, T1> state, Exception exception) => state.ToString();

		private readonly LogValuesFormatter _formatter;

		private readonly T0 _value0;

		private readonly T1 _value1;

		public KeyValuePair<string, object> this[int index] => index switch
		{
			0 => new KeyValuePair<string, object>(_formatter.ValueNames[0], _value0), 
			1 => new KeyValuePair<string, object>(_formatter.ValueNames[1], _value1), 
			2 => new KeyValuePair<string, object>("{OriginalFormat}", _formatter.OriginalFormat), 
			_ => throw new IndexOutOfRangeException("index"), 
		};

		public int Count => 3;

		public LogValues(LogValuesFormatter formatter, T0 value0, T1 value1)
		{
			_formatter = formatter;
			_value0 = value0;
			_value1 = value1;
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			int i = 0;
			while (i < Count)
			{
				yield return this[i];
				int num = i + 1;
				i = num;
			}
		}

		public override string ToString()
		{
			return _formatter.Format(_value0, _value1);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	private readonly struct LogValues<T0, T1, T2> : IReadOnlyList<KeyValuePair<string, object>>, IReadOnlyCollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable
	{
		public static readonly Func<LogValues<T0, T1, T2>, Exception, string> Callback = (LogValues<T0, T1, T2> state, Exception exception) => state.ToString();

		private readonly LogValuesFormatter _formatter;

		private readonly T0 _value0;

		private readonly T1 _value1;

		private readonly T2 _value2;

		public int Count => 4;

		public KeyValuePair<string, object> this[int index] => index switch
		{
			0 => new KeyValuePair<string, object>(_formatter.ValueNames[0], _value0), 
			1 => new KeyValuePair<string, object>(_formatter.ValueNames[1], _value1), 
			2 => new KeyValuePair<string, object>(_formatter.ValueNames[2], _value2), 
			3 => new KeyValuePair<string, object>("{OriginalFormat}", _formatter.OriginalFormat), 
			_ => throw new IndexOutOfRangeException("index"), 
		};

		public LogValues(LogValuesFormatter formatter, T0 value0, T1 value1, T2 value2)
		{
			_formatter = formatter;
			_value0 = value0;
			_value1 = value1;
			_value2 = value2;
		}

		public override string ToString()
		{
			return _formatter.Format(_value0, _value1, _value2);
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			int i = 0;
			while (i < Count)
			{
				yield return this[i];
				int num = i + 1;
				i = num;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	private readonly struct LogValues<T0, T1, T2, T3> : IReadOnlyList<KeyValuePair<string, object>>, IReadOnlyCollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable
	{
		public static readonly Func<LogValues<T0, T1, T2, T3>, Exception, string> Callback = (LogValues<T0, T1, T2, T3> state, Exception exception) => state.ToString();

		private readonly LogValuesFormatter _formatter;

		private readonly T0 _value0;

		private readonly T1 _value1;

		private readonly T2 _value2;

		private readonly T3 _value3;

		public int Count => 5;

		public KeyValuePair<string, object> this[int index] => index switch
		{
			0 => new KeyValuePair<string, object>(_formatter.ValueNames[0], _value0), 
			1 => new KeyValuePair<string, object>(_formatter.ValueNames[1], _value1), 
			2 => new KeyValuePair<string, object>(_formatter.ValueNames[2], _value2), 
			3 => new KeyValuePair<string, object>(_formatter.ValueNames[3], _value3), 
			4 => new KeyValuePair<string, object>("{OriginalFormat}", _formatter.OriginalFormat), 
			_ => throw new IndexOutOfRangeException("index"), 
		};

		public LogValues(LogValuesFormatter formatter, T0 value0, T1 value1, T2 value2, T3 value3)
		{
			_formatter = formatter;
			_value0 = value0;
			_value1 = value1;
			_value2 = value2;
			_value3 = value3;
		}

		private object[] ToArray()
		{
			return new object[4] { _value0, _value1, _value2, _value3 };
		}

		public override string ToString()
		{
			return _formatter.Format(ToArray());
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			int i = 0;
			while (i < Count)
			{
				yield return this[i];
				int num = i + 1;
				i = num;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	private readonly struct LogValues<T0, T1, T2, T3, T4> : IReadOnlyList<KeyValuePair<string, object>>, IReadOnlyCollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable
	{
		public static readonly Func<LogValues<T0, T1, T2, T3, T4>, Exception, string> Callback = (LogValues<T0, T1, T2, T3, T4> state, Exception exception) => state.ToString();

		private readonly LogValuesFormatter _formatter;

		private readonly T0 _value0;

		private readonly T1 _value1;

		private readonly T2 _value2;

		private readonly T3 _value3;

		private readonly T4 _value4;

		public int Count => 6;

		public KeyValuePair<string, object> this[int index] => index switch
		{
			0 => new KeyValuePair<string, object>(_formatter.ValueNames[0], _value0), 
			1 => new KeyValuePair<string, object>(_formatter.ValueNames[1], _value1), 
			2 => new KeyValuePair<string, object>(_formatter.ValueNames[2], _value2), 
			3 => new KeyValuePair<string, object>(_formatter.ValueNames[3], _value3), 
			4 => new KeyValuePair<string, object>(_formatter.ValueNames[4], _value4), 
			5 => new KeyValuePair<string, object>("{OriginalFormat}", _formatter.OriginalFormat), 
			_ => throw new IndexOutOfRangeException("index"), 
		};

		public LogValues(LogValuesFormatter formatter, T0 value0, T1 value1, T2 value2, T3 value3, T4 value4)
		{
			_formatter = formatter;
			_value0 = value0;
			_value1 = value1;
			_value2 = value2;
			_value3 = value3;
			_value4 = value4;
		}

		private object[] ToArray()
		{
			return new object[5] { _value0, _value1, _value2, _value3, _value4 };
		}

		public override string ToString()
		{
			return _formatter.Format(ToArray());
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			int i = 0;
			while (i < Count)
			{
				yield return this[i];
				int num = i + 1;
				i = num;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	private readonly struct LogValues<T0, T1, T2, T3, T4, T5> : IReadOnlyList<KeyValuePair<string, object>>, IReadOnlyCollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable
	{
		public static readonly Func<LogValues<T0, T1, T2, T3, T4, T5>, Exception, string> Callback = (LogValues<T0, T1, T2, T3, T4, T5> state, Exception exception) => state.ToString();

		private readonly LogValuesFormatter _formatter;

		private readonly T0 _value0;

		private readonly T1 _value1;

		private readonly T2 _value2;

		private readonly T3 _value3;

		private readonly T4 _value4;

		private readonly T5 _value5;

		public int Count => 7;

		public KeyValuePair<string, object> this[int index] => index switch
		{
			0 => new KeyValuePair<string, object>(_formatter.ValueNames[0], _value0), 
			1 => new KeyValuePair<string, object>(_formatter.ValueNames[1], _value1), 
			2 => new KeyValuePair<string, object>(_formatter.ValueNames[2], _value2), 
			3 => new KeyValuePair<string, object>(_formatter.ValueNames[3], _value3), 
			4 => new KeyValuePair<string, object>(_formatter.ValueNames[4], _value4), 
			5 => new KeyValuePair<string, object>(_formatter.ValueNames[5], _value5), 
			6 => new KeyValuePair<string, object>("{OriginalFormat}", _formatter.OriginalFormat), 
			_ => throw new IndexOutOfRangeException("index"), 
		};

		public LogValues(LogValuesFormatter formatter, T0 value0, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
		{
			_formatter = formatter;
			_value0 = value0;
			_value1 = value1;
			_value2 = value2;
			_value3 = value3;
			_value4 = value4;
			_value5 = value5;
		}

		private object[] ToArray()
		{
			return new object[6] { _value0, _value1, _value2, _value3, _value4, _value5 };
		}

		public override string ToString()
		{
			return _formatter.Format(ToArray());
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			int i = 0;
			while (i < Count)
			{
				yield return this[i];
				int num = i + 1;
				i = num;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	public static Func<ILogger, IDisposable> DefineScope(string formatString)
	{
		LogValuesFormatter formatter = CreateLogValuesFormatter(formatString, 0);
		LogValues logValues = new LogValues(formatter);
		return (ILogger logger) => logger.BeginScope(logValues);
	}

	public static Func<ILogger, T1, IDisposable> DefineScope<T1>(string formatString)
	{
		LogValuesFormatter formatter = CreateLogValuesFormatter(formatString, 1);
		return (ILogger logger, T1 arg1) => logger.BeginScope(new LogValues<T1>(formatter, arg1));
	}

	public static Func<ILogger, T1, T2, IDisposable> DefineScope<T1, T2>(string formatString)
	{
		LogValuesFormatter formatter = CreateLogValuesFormatter(formatString, 2);
		return (ILogger logger, T1 arg1, T2 arg2) => logger.BeginScope(new LogValues<T1, T2>(formatter, arg1, arg2));
	}

	public static Func<ILogger, T1, T2, T3, IDisposable> DefineScope<T1, T2, T3>(string formatString)
	{
		LogValuesFormatter formatter = CreateLogValuesFormatter(formatString, 3);
		return (ILogger logger, T1 arg1, T2 arg2, T3 arg3) => logger.BeginScope(new LogValues<T1, T2, T3>(formatter, arg1, arg2, arg3));
	}

	public static Action<ILogger, Exception> Define(LogLevel logLevel, EventId eventId, string formatString)
	{
		LogValuesFormatter formatter = CreateLogValuesFormatter(formatString, 0);
		return delegate(ILogger logger, Exception exception)
		{
			if (logger.IsEnabled(logLevel))
			{
				logger.Log(logLevel, eventId, new LogValues(formatter), exception, LogValues.Callback);
			}
		};
	}

	public static Action<ILogger, T1, Exception> Define<T1>(LogLevel logLevel, EventId eventId, string formatString)
	{
		LogValuesFormatter formatter = CreateLogValuesFormatter(formatString, 1);
		return delegate(ILogger logger, T1 arg1, Exception exception)
		{
			if (logger.IsEnabled(logLevel))
			{
				Log(logger, arg1, exception);
			}
		};
		void Log(ILogger logger, T1 arg1, Exception exception)
		{
			logger.Log(logLevel, eventId, new LogValues<T1>(formatter, arg1), exception, LogValues<T1>.Callback);
		}
	}

	public static Action<ILogger, T1, T2, Exception> Define<T1, T2>(LogLevel logLevel, EventId eventId, string formatString)
	{
		LogValuesFormatter formatter = CreateLogValuesFormatter(formatString, 2);
		return delegate(ILogger logger, T1 arg1, T2 arg2, Exception exception)
		{
			if (logger.IsEnabled(logLevel))
			{
				Log(logger, arg1, arg2, exception);
			}
		};
		void Log(ILogger logger, T1 arg1, T2 arg2, Exception exception)
		{
			logger.Log(logLevel, eventId, new LogValues<T1, T2>(formatter, arg1, arg2), exception, LogValues<T1, T2>.Callback);
		}
	}

	public static Action<ILogger, T1, T2, T3, Exception> Define<T1, T2, T3>(LogLevel logLevel, EventId eventId, string formatString)
	{
		LogValuesFormatter formatter = CreateLogValuesFormatter(formatString, 3);
		return delegate(ILogger logger, T1 arg1, T2 arg2, T3 arg3, Exception exception)
		{
			if (logger.IsEnabled(logLevel))
			{
				Log(logger, arg1, arg2, arg3, exception);
			}
		};
		void Log(ILogger logger, T1 arg1, T2 arg2, T3 arg3, Exception exception)
		{
			logger.Log(logLevel, eventId, new LogValues<T1, T2, T3>(formatter, arg1, arg2, arg3), exception, LogValues<T1, T2, T3>.Callback);
		}
	}

	public static Action<ILogger, T1, T2, T3, T4, Exception> Define<T1, T2, T3, T4>(LogLevel logLevel, EventId eventId, string formatString)
	{
		LogValuesFormatter formatter = CreateLogValuesFormatter(formatString, 4);
		return delegate(ILogger logger, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Exception exception)
		{
			if (logger.IsEnabled(logLevel))
			{
				Log(logger, arg1, arg2, arg3, arg4, exception);
			}
		};
		void Log(ILogger logger, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Exception exception)
		{
			logger.Log(logLevel, eventId, new LogValues<T1, T2, T3, T4>(formatter, arg1, arg2, arg3, arg4), exception, LogValues<T1, T2, T3, T4>.Callback);
		}
	}

	public static Action<ILogger, T1, T2, T3, T4, T5, Exception> Define<T1, T2, T3, T4, T5>(LogLevel logLevel, EventId eventId, string formatString)
	{
		LogValuesFormatter formatter = CreateLogValuesFormatter(formatString, 5);
		return delegate(ILogger logger, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Exception exception)
		{
			if (logger.IsEnabled(logLevel))
			{
				logger.Log(logLevel, eventId, new LogValues<T1, T2, T3, T4, T5>(formatter, arg1, arg2, arg3, arg4, arg5), exception, LogValues<T1, T2, T3, T4, T5>.Callback);
			}
		};
	}

	public static Action<ILogger, T1, T2, T3, T4, T5, T6, Exception> Define<T1, T2, T3, T4, T5, T6>(LogLevel logLevel, EventId eventId, string formatString)
	{
		LogValuesFormatter formatter = CreateLogValuesFormatter(formatString, 6);
		return delegate(ILogger logger, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Exception exception)
		{
			if (logger.IsEnabled(logLevel))
			{
				logger.Log(logLevel, eventId, new LogValues<T1, T2, T3, T4, T5, T6>(formatter, arg1, arg2, arg3, arg4, arg5, arg6), exception, LogValues<T1, T2, T3, T4, T5, T6>.Callback);
			}
		};
	}

	private static LogValuesFormatter CreateLogValuesFormatter(string formatString, int expectedNamedParameterCount)
	{
		LogValuesFormatter logValuesFormatter = new LogValuesFormatter(formatString);
		int count = logValuesFormatter.ValueNames.Count;
		if (count != expectedNamedParameterCount)
		{
			throw new ArgumentException(Resource.FormatUnexpectedNumberOfNamedParameters(formatString, expectedNamedParameterCount, count));
		}
		return logValuesFormatter;
	}
}
