using GamersWorld.Application.Contracts.Data;
using GamersWorld.Application.Contracts.Document;
using GamersWorld.Application.Contracts.Events;
using GamersWorld.Application.Contracts.MessageQueue;
using GamersWorld.Domain.Entity;
using GamersWorld.Domain.Enums;
using GamersWorld.Domain.Requests;
using GamersWorld.Domain.Responses;
using Microsoft.Extensions.Logging;

namespace GamersWorld.Application.Document;

public class TableSaver(
    ILogger<FileSaver> logger
    , IEventQueueService eventQueueService
    , IReportDataRepository reportDataRepository
    , IReportDocumentDataRepository reportDocumentDataRepository)
    : IDocumentWriter
{
    private readonly ILogger<FileSaver> _logger = logger;
    private readonly IEventQueueService _eventQueueService = eventQueueService;
    private readonly IReportDataRepository _reportDataRepository = reportDataRepository;
    private readonly IReportDocumentDataRepository _reportDocumentDataRepository = reportDocumentDataRepository;

    public async Task<BusinessResponse> SaveAsync(ReportSaveRequest payload)
    {
        if (payload == null || payload.Content == null)
        {
            _logger.LogError("Paylod or content is null");
            return new BusinessResponse
            {
                Status = Status.Fail,
                Message = "Payload or content is null"
            };
        }

        try
        {
            var insertedId = await _reportDataRepository.CreateReportAsync(new Report
            {
                TraceId = payload.TraceId,
                EmployeeId = payload.EmployeeId,
                Title = payload.Title,
                Expression = payload.Expression,
                DocumentId = payload.DocumentId,
                InsertTime = payload.InsertTime,
                ExpireTime = payload.ExpireTime
            });

            await _reportDocumentDataRepository.CreateReportDocumentAsync(new ReportDocument
            {
                ReportId = insertedId,
                Content = payload.Content
            });

            var reportIsHereEvent = new ReportIsHereEvent
            {
                EventData = new BaseEventData
                {
                    TraceId = payload.TraceId,
                    Time = DateTime.Now
                },
                Title = payload.Title,
                CreatedReportId = payload.DocumentId,
                EmployeeId = payload.EmployeeId,
            };
            _eventQueueService.PublishEvent(reportIsHereEvent);

            return new BusinessResponse
            {
                Status = Status.DocumentSaved,
                Message = $"{payload.Content.Length} bytes saved to database table."
            };
        }
        catch (Exception excp)
        {
            _logger.LogError(excp, "Error on document saving!");
            return new BusinessResponse
            {
                Status = Status.Fail,
                Message = $"Exception. {excp.Message}"
            };
        }
    }
}