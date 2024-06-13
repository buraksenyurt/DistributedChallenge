namespace Kahin.Common.Services;
public interface ISecretStoreService
{
    Task<string> GetSecretAsync(string secretName);
}