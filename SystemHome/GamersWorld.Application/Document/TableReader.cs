using GamersWorld.Application.Contracts.Document;
using GamersWorld.Domain.Enums;
using GamersWorld.Domain.Requests;
using GamersWorld.Domain.Responses;
using Microsoft.Extensions.Logging;

namespace GamersWorld.Application.Document;

public class TableReader(ILogger<FileSaver> logger, IDocumentDataRepository documentDataRepository)
    : IDocumentReader
{
    private readonly ILogger<FileSaver> _logger = logger;
    private readonly IDocumentDataRepository _documentDataRepository = documentDataRepository;

    public async Task<BusinessResponse> GetLength(GenericDocumentRequest payload)
    {
        try
        {
            var contentLength = await _documentDataRepository.GetDocumentLength(payload);
            return new BusinessResponse
            {
                StatusCode = StatusCode.DocumentReadable,
                Message = $"{contentLength} bytes length document is ready."
            };
        }
        catch (Exception excp)
        {
            _logger.LogError(excp, "Error on document reading!");
            return new BusinessResponse
            {
                StatusCode = StatusCode.Fail,
                Message = $"Exception. {excp.Message}"
            };
        }
    }
}