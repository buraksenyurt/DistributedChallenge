using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.Logging;

namespace Kahin.Common.Services;
public class SecretStoreService
    : ISecretStoreService
{
    private readonly AmazonSecretsManagerClient _secretsManager;
    private readonly ILogger<SecretStoreService> _logger;
    public SecretStoreService(ILogger<SecretStoreService> logger)
    {
        _logger = logger;
        _secretsManager = new AmazonSecretsManagerClient(
                            new AmazonSecretsManagerConfig
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
            _logger.LogError("Errors on getting secret value.");
            throw new InvalidOperationException($"Error fetching secret {secretName}: {ex.Message}", ex);
        }
    }
}