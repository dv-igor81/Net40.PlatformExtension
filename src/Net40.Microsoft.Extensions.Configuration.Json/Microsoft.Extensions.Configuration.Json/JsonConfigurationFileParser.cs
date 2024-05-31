using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Microsoft.Extensions.Configuration.Json;

internal class JsonConfigurationFileParser
{
	private readonly IDictionary<string, string> _data = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

	private readonly Stack<string> _context = new Stack<string>();

	private string _currentPath;

	private JsonConfigurationFileParser()
	{
	}

	public static IDictionary<string, string> Parse(Stream input)
	{
		return new JsonConfigurationFileParser().ParseStream(input);
	}

	private IDictionary<string, string> ParseStream(Stream input)
	{
		_data.Clear();
		JsonDocumentOptions jsonDocumentOptions2 = default(JsonDocumentOptions);
		jsonDocumentOptions2.CommentHandling = JsonCommentHandling.Skip;
		jsonDocumentOptions2.AllowTrailingCommas = true;
		JsonDocumentOptions jsonDocumentOptions = jsonDocumentOptions2;
		using (StreamReader reader = new StreamReader(input))
		{
			using JsonDocument doc = JsonDocument.Parse(reader.ReadToEnd(), jsonDocumentOptions);
			if (doc.RootElement.ValueKind != JsonValueKind.Object)
			{
				throw new FormatException("Resources.FormatError_UnsupportedJSONToken(doc.RootElement.ValueKind)");
			}
			VisitElement(doc.RootElement);
		}
		return _data;
	}

	private void VisitElement(JsonElement element)
	{
		foreach (JsonProperty property in element.EnumerateObject())
		{
			EnterContext(property.Name);
			VisitValue(property.Value);
			ExitContext();
		}
	}

	private void VisitValue(JsonElement value)
	{
		switch (value.ValueKind)
		{
		case JsonValueKind.Object:
			VisitElement(value);
			break;
		case JsonValueKind.Array:
		{
			int index = 0;
			{
				foreach (JsonElement arrayElement in value.EnumerateArray())
				{
					EnterContext(index.ToString());
					VisitValue(arrayElement);
					ExitContext();
					index++;
				}
				break;
			}
		}
		case JsonValueKind.String:
		case JsonValueKind.Number:
		case JsonValueKind.True:
		case JsonValueKind.False:
		case JsonValueKind.Null:
		{
			string key = _currentPath;
			if (_data.ContainsKey(key))
			{
				throw new FormatException("Resources.FormatError_KeyIsDuplicated(key)");
			}
			_data[key] = value.ToString();
			break;
		}
		default:
			throw new FormatException("Resources.FormatError_UnsupportedJSONToken(value.ValueKind)");
		}
	}

	private void EnterContext(string context)
	{
		_context.Push(context);
		_currentPath = ConfigurationPath.Combine(_context.Reverse());
	}

	private void ExitContext()
	{
		_context.Pop();
		_currentPath = ConfigurationPath.Combine(_context.Reverse());
	}
}
