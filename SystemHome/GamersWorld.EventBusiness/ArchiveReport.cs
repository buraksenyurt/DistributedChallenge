using Microsoft.Extensions.Logging;
using GamersWorld.Application.Contracts.Events;
using GamersWorld.Application.Contracts.Document;

namespace GamersWorld.EventBusiness;

public class ArchiveReport(ILogger<ArchiveReport> logger, IDocumentRepository documentRepository, IDocumentWriter documentWriter) : IEventDriver<ArchiveReportEvent>
{
    private readonly ILogger<ArchiveReport> _logger = logger;
    private readonly IDocumentRepository _documentRepository = documentRepository;
    private readonly IDocumentWriter _documentWriter = documentWriter;

    public async Task Execute(ArchiveReportEvent appEvent)
    {
        var request = new Domain.Requests.DocumentReadRequest
        {
            DocumentId = appEvent.DocumentId,
        };
        var doc = await _documentRepository.ReadDocumentAsync(request);
        if (doc == null || doc.Content == null)
        {
            _logger.LogWarning("{DocumentId} content not found", appEvent.DocumentId);
            return;
        }

        var writeResponse = await _documentWriter.SaveAsync(new Domain.Requests.DocumentSaveRequest
        {
            DocumentId = appEvent.DocumentId,
            Content = doc.Content,
        });
        if (writeResponse.StatusCode != Domain.Enums.StatusCode.Success)
        {
            _logger.LogWarning("{DocumentId} save operation failed", doc.DocumentId);
            return;
        }

        var deleteResult = await _documentRepository.DeleteDocumentByIdAsync(request);
        if (deleteResult == 1)
        {
            _logger.LogWarning("{Document} is archiving to target", appEvent.DocumentId);
        }
        else
        {
            _logger.LogWarning("{DocumentId} delete from db operation failed", doc.DocumentId);
        }
    }
}
