using GamersWorld.Business.Contracts;
using GamersWorld.Common.Enums;
using GamersWorld.Common.Requests;
using GamersWorld.Common.Responses;
using GamersWorld.Repository;
using Microsoft.Extensions.Logging;

namespace GamersWorld.Business.Concretes;

public class TableReader(ILogger<FileSaver> logger, IDocumentRepository documentRepository)
    : IDocumentReader
{
    private readonly ILogger<FileSaver> _logger = logger;
    private readonly IDocumentRepository _documentRepository = documentRepository;

    public async Task<BusinessResponse> ReadAsync(DocumentReadRequest payload)
    {
        try
        {
            var content = await _documentRepository.ReadDocumentAsync(payload);

            // QUESTION Rapor dosyasını taşıyan içeriği Data özelliği üzerinden sistemde taşımak doğru mu?
            return new BusinessResponse
            {
                StatusCode = StatusCode.DocumentRead,
                Message = $"{content.Length} bytes read from database table.",
                Data = content
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