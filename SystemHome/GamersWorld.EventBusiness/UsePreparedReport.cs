using GamersWorld.Business.Contracts;
using GamersWorld.Common.Requests;
using GamersWorld.Events;
using GamersWorld.SDK;
using Microsoft.Extensions.Logging;

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

        var response = await _documentReader.ReadAsync(new DocumentReadRequest
        {
            DocumentId = appEvent.CreatedReportId,
            TraceId = appEvent.TraceId
        });
        // QUESTION Sorunlu bir yer. Data byte[]'a convert edilemeyen bir şeyse ne olacak?
        if (response != null && response.Data != null)
        {
            _logger.LogInformation("Catched {Length} bytes document.", ((byte[])response.Data).Length);
            await _notificationService.PushAsync(appEvent.ClientId.ToString(), $"{appEvent.CreatedReportId} is ready!");
        }
    }
}
