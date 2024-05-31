using System;
using System.IO;
using System.Text.Json;

namespace Microsoft.Extensions.Configuration.Json;

public class JsonConfigurationProvider : FileConfigurationProvider
{
	public JsonConfigurationProvider(JsonConfigurationSource source)
		: base(source)
	{
	}

	public override void Load(Stream stream)
	{
		try
		{
			base.Data = JsonConfigurationFileParser.Parse(stream);
		}
		catch (JsonException e)
		{
			throw new FormatException("Resources.Error_JSONParseError", e);
		}
	}
}
