using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace Kahin.Common.Services;
public class SecretStoreService
    :ISecretStoreService
{
    private readonly IAmazonSecretsManager _secretsManager;

    public SecretStoreService()
    {
        _secretsManager = new AmazonSecretsManagerClient(new AmazonSecretsManagerConfig
        {
            ServiceURL = "http://localhost:4566",
            AuthenticationRegion = "eu-west-1"
        });
    }

    public async Task<string> GetSecretAsync(string secretName)
    {
        try
        {
            var request = new GetSecretValueRequest
            {
                SecretId = secretName
            };

            var response = await _secretsManager.GetSecretValueAsync(request);
            return response.SecretString;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error fetching secret {secretName}: {ex.Message}", ex);
        }
    }
}