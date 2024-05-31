using System;

namespace Microsoft.Extensions.Configuration.UserSecrets;

[AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
public class UserSecretsIdAttribute : Attribute
{
	public string UserSecretsId { get; }

	public UserSecretsIdAttribute(string userSecretId)
	{
		if (string.IsNullOrEmpty(userSecretId))
		{
			throw new ArgumentException("Resources.Common_StringNullOrEmpty", "userSecretId");
		}
		UserSecretsId = userSecretId;
	}
}
