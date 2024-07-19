using GamersWorld.Application.Contracts.Document;
using GamersWorld.Application.Contracts.Events;
using GamersWorld.Application.Contracts.MessageQueue;
using GamersWorld.Domain.Enums;
using GamersWorld.Domain.Requests;
using GamersWorld.Domain.Responses;
using Microsoft.Extensions.Logging;

namespace GamersWorld.Application.Document;

public class TableSaver(ILogger<FileSaver> logger, IEventQueueService eventQueueService, IDocumentDataRepository documentRepository)
    : IDocumentWriter
{
    private readonly ILogger<FileSaver> _logger = logger;
    private readonly IEventQueueService _eventQueueService = eventQueueService;
    private readonly IDocumentDataRepository _documentRepository = documentRepository;

    public async Task<BusinessResponse> SaveAsync(DocumentSaveRequest payload)
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
            var insertedId = await _documentRepository.InsertDocumentAsync(payload);

            var reportIsHereEvent = new ReportIsHereEvent
            {
                Time = DateTime.Now,
                Title = payload.ReportTitle,
                TraceId = payload.TraceId,
                CreatedReportId = payload.DocumentId,
                EmployeeId = payload.EmployeeId,
            };
            _eventQueueService.PublishEvent(reportIsHereEvent);

            return new BusinessResponse
            {
                StatusCode = StatusCode.DocumentSaved,
                Message = $"{payload.Content.Length} bytes saved to database table."
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
}