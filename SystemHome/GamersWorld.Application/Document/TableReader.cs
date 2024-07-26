using GamersWorld.Application.Contracts.Data;
using GamersWorld.Application.Contracts.Document;
using GamersWorld.Domain.Enums;
using GamersWorld.Domain.Requests;
using GamersWorld.Domain.Responses;
using Microsoft.Extensions.Logging;

namespace GamersWorld.Application.Document;

public class TableReader(ILogger<FileSaver> logger, IReportDocumentDataRepository reportDocumentDataRepository)
    : IDocumentReader
{
    private readonly ILogger<FileSaver> _logger = logger;
    private readonly IReportDocumentDataRepository _reportDocumentDataRepository = reportDocumentDataRepository;

    public async Task<BusinessResponse> GetLength(GenericDocumentRequest payload)
    {
        try
        {
            var contentLength = await _reportDocumentDataRepository.GetDocumentLength(payload.DocumentId);
            return new BusinessResponse
            {
                Status = Status.DocumentReadable,
                Message = $"{contentLength} bytes length document is ready."
            };
        }
        catch (Exception excp)
        {
            _logger.LogError(excp, "Error on document reading!");
            return new BusinessResponse
            {
                Status = Status.Fail,
                Message = $"Exception. {excp.Message}"
            };
        }
    }
}