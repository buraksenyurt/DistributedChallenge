using FluentFTP;
using GamersWorld.Application.Contracts.Document;
using GamersWorld.Application.Contracts.MessageQueue;
using GamersWorld.Domain.Requests;
using GamersWorld.Domain.Responses;
using Microsoft.Extensions.Logging;
using SecretsAgent;

namespace GamersWorld.Application.Document;

public class FtpReader(ILogger<FileSaver> logger, ISecretStoreService secretStoreService, IEventQueueService eventQueueService)
    : IDocumentReader
{
    private readonly ILogger<FileSaver> _logger = logger;
    private readonly IEventQueueService _eventQueueService = eventQueueService;
    private readonly ISecretStoreService _secretStoreService = secretStoreService;

    public Task<BusinessResponse> GetLength(DocumentReadRequest payload)
    {
        throw new NotImplementedException();
    }

    private async Task<FtpStatus> UploadFileAsync(byte[] content, string fileName)
    {
        var ftpServer = await _secretStoreService.GetSecretAsync("FtpServerAddress");
        var ftpUsername = await _secretStoreService.GetSecretAsync("FtpUsername");
        var ftpPassword = await _secretStoreService.GetSecretAsync("FtpPassword");

        var token = new CancellationToken();
        using var client = new AsyncFtpClient(ftpServer, ftpUsername, ftpPassword);
        await client.Connect(token);

        var ftpStatus = await client.UploadBytes(content, $"/documents/{fileName}", token: token);
        _logger.LogInformation("Upload File Complete, status {StatusDescription}", ftpStatus.ToString());

        return ftpStatus;
    }
}