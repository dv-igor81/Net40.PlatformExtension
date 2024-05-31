using System;
using System.IO;
using Net40.Microsoft.Extensions.Configuration.UserSecrets;

namespace Microsoft.Extensions.Configuration.UserSecrets;

public class PathHelper
{
	internal const string SecretsFileName = "secrets.json";

	public static string GetSecretsPathFromSecretsId(string userSecretsId)
	{
		if (string.IsNullOrEmpty(userSecretsId))
		{
			throw new ArgumentException(Resources.Common_StringNullOrEmpty, "userSecretsId");
		}
		int badCharIndex = userSecretsId.IndexOfAny(Path.GetInvalidFileNameChars());
		if (badCharIndex != -1)
		{
			throw new InvalidOperationException(string.Format(Resources.Error_Invalid_Character_In_UserSecrets_Id, userSecretsId[badCharIndex], badCharIndex));
		}
		string appData = Environment.GetEnvironmentVariable("APPDATA");
		string root = appData ?? Environment.GetEnvironmentVariable("HOME") ?? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		if (string.IsNullOrEmpty(root))
		{
			throw new InvalidOperationException("Could not determine an appropriate location for storing user secrets. Set the DOTNET_USER_SECRETS_FALLBACK_DIR environment variable to a folder where user secrets should be stored.");
		}
		return (!string.IsNullOrEmpty(appData)) ? Path.Combine(root, "Microsoft", "UserSecrets", userSecretsId, "secrets.json") : Path.Combine(root, ".microsoft", "usersecrets", userSecretsId, "secrets.json");
	}
}
