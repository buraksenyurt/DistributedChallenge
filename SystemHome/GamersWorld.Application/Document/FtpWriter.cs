using FluentFTP;
using GamersWorld.Application.Contracts.Document;
using GamersWorld.Domain.Enums;
using GamersWorld.Domain.Requests;
using GamersWorld.Domain.Responses;
using Microsoft.Extensions.Logging;
using SecretsAgent;

namespace GamersWorld.Application.Document;

public class FtpWriter(ILogger<FileSaver> logger, ISecretStoreService secretStoreService)
    : IDocumentWriter
{
    private readonly ILogger<FileSaver> _logger = logger;
    private readonly ISecretStoreService _secretStoreService = secretStoreService;

    public async Task<BusinessResponse> SaveAsync(ReportSaveRequest payload)
    {
        if (payload == null || payload.Content == null)
        {
            _logger.LogError("Paylod or content is null");
            return new BusinessResponse
            {
                StatusCode = StatusCode.Fail,
                Message = "Payload or content is null"
            };
        }

        try
        {
            var response = await UploadFileAsync(payload.Content, $"{payload.DocumentId}.csv");
            if (response == FtpStatus.Success)
            {
                return new BusinessResponse
                {
                    StatusCode = StatusCode.DocumentUploaded,
                    Message = $"{payload.Content.Length} bytes saved."
                };
            }

            return new BusinessResponse
            {
                StatusCode = StatusCode.Fail,
                Message = "Ftp save error"
            };
        }
        catch (Exception excp)
        {
            _logger.LogError(excp, "Error on document saving!");
            return new BusinessResponse
            {
                StatusCode = StatusCode.Fail,
                Message = $"Exception. {excp.Message}"
            };
        }
    }

    private async Task<FtpStatus> UploadFileAsync(byte[] content, string fileName)
    {
        var ftpServer = await _secretStoreService.GetSecretAsync("FtpServerAddress");
        var ftpUsername = await _secretStoreService.GetSecretAsync("FtpUsername");
        var ftpPassword = await _secretStoreService.GetSecretAsync("FtpPassword");

        var token = new CancellationToken();
        using var client = new AsyncFtpClient(ftpServer, ftpUsername, ftpPassword);
        await client.Connect(token);

        var rootFolder = "/home/ftpuser/documents";

        _logger.LogInformation("Checking the directory exists.");
        if (!await client.DirectoryExists(rootFolder, token))
        {
            _logger.LogInformation("Directory does not exist. Creating it.");
            await client.CreateDirectory(rootFolder, token);
        }

        var ftpStatus = await client.UploadBytes(content, $"{rootFolder}/{fileName}", token: token);
        _logger.LogInformation("Upload File to {TargetFolder} Complete. Status : {StatusDescription}", rootFolder, ftpStatus.ToString());

        return ftpStatus;
    }
}