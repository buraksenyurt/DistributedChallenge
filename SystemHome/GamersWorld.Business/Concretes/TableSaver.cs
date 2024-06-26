using GamersWorld.Business.Contracts;
using GamersWorld.Common.Enums;
using GamersWorld.Common.Requests;
using GamersWorld.Common.Responses;
using GamersWorld.Events;
using GamersWorld.MQ;
using GamersWorld.Repository;
using Microsoft.Extensions.Logging;

namespace GamersWorld.Business.Concretes;

public class TableSaver(ILogger<FileSaver> logger, IEventQueueService eventQueueService, IDocumentRepository documentRepository)
    : IDocumentWriter
{
    private readonly ILogger<FileSaver> _logger = logger;
    private readonly IEventQueueService _eventQueueService = eventQueueService;
    private readonly IDocumentRepository _documentRepository = documentRepository;

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