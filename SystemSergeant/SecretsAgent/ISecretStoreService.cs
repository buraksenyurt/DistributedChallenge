namespace SecretsAgent;
public interface ISecretStoreService
{
    Task<string> GetSecretAsync(string secretName);
}