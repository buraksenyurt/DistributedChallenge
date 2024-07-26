using FluentFTP;
using GamersWorld.Application.Contracts.Document;
using GamersWorld.Domain.Enums;
using GamersWorld.Domain.Requests;
using GamersWorld.Domain.Responses;
using Microsoft.Extensions.Logging;
using SecretsAgent;

namespace GamersWorld.Application.Document;

public class FtpDestroyer(ILogger<FtpDestroyer> logger, ISecretStoreService secretStoreService)
    : IDocumentDestroyer
{
    private readonly ILogger<FtpDestroyer> _logger = logger;
    private readonly ISecretStoreService _secretStoreService = secretStoreService;

    public async Task<BusinessResponse> DeleteAsync(GenericDocumentRequest payload)
    {
        var ftpServer = await _secretStoreService.GetSecretAsync("FtpServerAddress");
        var ftpUsername = await _secretStoreService.GetSecretAsync("FtpUsername");
        var ftpPassword = await _secretStoreService.GetSecretAsync("FtpPassword");

        var token = new CancellationToken();
        using var client = new AsyncFtpClient(ftpServer, ftpUsername, ftpPassword);
        await client.Connect(token);

        var fileName = $"/home/ftpuser/documents/{payload.DocumentId}.csv";
        try
        {
            if (await client.FileExists(fileName))
            {
                await client.DeleteFile(fileName, token: token);
                _logger.LogInformation("{DocumentId} has been deleted", payload.DocumentId);
                return new BusinessResponse
                {
                    Status = Status.Success,
                    Message = $"{payload.DocumentId} has been deleted"
                };
            } else
            {
                _logger.LogWarning("{DocumentId} not found on ftp", payload.DocumentId);
                return new BusinessResponse
                {
                    Status = Status.DocumentNotFound,
                    Message = $"{payload.DocumentId} not found on ftp"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception on Ftp file delete operation");
            return new BusinessResponse
            {
                Status = Status.Fail,
                Message = ex.Message
            };
        }
    }
}