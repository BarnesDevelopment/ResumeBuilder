using Infisical.Sdk;

namespace ResumeAPI.Helpers;

public class InfisicalSecretsClient
{
    private readonly InfisicalClient _client;

    public InfisicalSecretsClient(string clientId, string clientSecret)
    {
        var settings = new ClientSettings
        {
            SiteUrl = "https://secrets.barnes7619.com",
            Auth = new AuthenticationOptions
            {
                UniversalAuth = new UniversalAuthMethod { ClientId = clientId, ClientSecret = clientSecret }
            }
        };

        _client = new InfisicalClient(settings);
    }

    public string GetPostgresConnectionString()
    {
        var options = new GetSecretOptions
        {
            SecretName = "POSTGRES_CONNECTION_STRING",
            Environment = "dev",
            ProjectId = "39828bb7-07bb-4f99-bc16-706caf452bde"
        };

        var secret = _client.GetSecret(options);
        return secret.SecretValue;
    }
}