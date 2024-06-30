using GamersWorld.Domain.Enums;
using GamersWorld.Domain.Requests;
using Microsoft.Extensions.Logging;
using GamersWorld.Application.Contracts.Events;
using GamersWorld.Application.Contracts.Document;
using GamersWorld.Application.Contracts.Notification;

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
        if (response != null && response.StatusCode == StatusCode.DocumentReadable)
        {
            _logger.LogInformation("{Message}", response.Message);
            //await _notificationService.PushAsync($"{appEvent.CreatedReportId} is ready!");

            await _notificationService.PushToUserAsync(appEvent.EmployeeId, $"{appEvent.CreatedReportId} is ready!");
        }
    }
}
