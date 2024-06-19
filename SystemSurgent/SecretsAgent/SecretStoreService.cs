using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace SecretsAgent;
public class SecretStoreService
    : ISecretStoreService
{
    private readonly AmazonSecretsManagerClient _secretsManager;
    private readonly ILogger<SecretStoreService> _logger;
    private readonly string environment;

    public SecretStoreService(ILogger<SecretStoreService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _secretsManager = new AmazonSecretsManagerClient(
            new AmazonSecretsManagerConfig
            {
                ServiceURL = "http://localhost:4566",
                AuthenticationRegion = "eu-west-1"
            });

        environment = configuration["Environment"] ?? "Development";
    }

    public async Task<string> GetSecretAsync(string secretName)
    {
        try
        {
            var request = new GetSecretValueRequest
            {
                SecretId = secretName
            };

            var listRequest = new ListSecretsRequest();
            var listResponse = await _secretsManager.ListSecretsAsync(listRequest);
            var secret = listResponse.SecretList
                .FirstOrDefault(s => s.Name == secretName
                                && s.Tags.Any(tag => tag.Key == "Environment" && tag.Value == environment))
                ?? throw new InvalidOperationException($"Secret {secretName} not found for environment {environment}");
            request.SecretId = secret.ARN;

            return (await _secretsManager.GetSecretValueAsync(request)).SecretString;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Environment}:{SecretName}", secretName, environment);
            throw new InvalidOperationException($"Error fetching {environment}:{secretName} {ex.Message}", ex);
        }
    }
}
