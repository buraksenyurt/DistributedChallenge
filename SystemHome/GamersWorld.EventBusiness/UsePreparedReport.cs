using GamersWorld.Domain.Enums;
using GamersWorld.Domain.Requests;
using Microsoft.Extensions.Logging;
using GamersWorld.Application.Contracts.Events;
using GamersWorld.Application.Contracts.Document;
using GamersWorld.Application.Contracts.Notification;
using System.Text.Json;
using GamersWorld.Domain.Dtos;

namespace GamersWorld.EventBusiness;

public class UsePreparedReport(ILogger<UsePreparedReport> logger, IDocumentReader documentReader, INotificationService notificationService)
    : IEventDriver<ReportIsHereEvent>
{
    private readonly ILogger<UsePreparedReport> _logger = logger;
    private readonly IDocumentReader _documentReader = documentReader;
    private readonly INotificationService _notificationService = notificationService;

    public async Task Execute(ReportIsHereEvent appEvent)
    {
        _logger.LogInformation("Document Accepted, Trace Id : {TraceId}, Document Id : {CreatedReportId}"
            , appEvent.TraceId, appEvent.CreatedReportId);

        var response = await _documentReader.GetLength(new DocumentReadRequest
        {
            DocumentId = appEvent.CreatedReportId,
            TraceId = appEvent.TraceId
        });
        if (response is { StatusCode: StatusCode.DocumentReadable })
        {
            _logger.LogInformation("{Message}", response.Message);

            var notificationData = new ReportNotification
            {
                DocumentId = appEvent.CreatedReportId,
                Content = appEvent.Title
            };

            await _notificationService.PushToUserAsync(appEvent.EmployeeId, JsonSerializer.Serialize(notificationData));
        }
    }
}