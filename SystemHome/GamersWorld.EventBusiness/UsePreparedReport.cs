using GamersWorld.Events;
using GamersWorld.SDK;
using Microsoft.Extensions.Logging;

namespace GamersWorld.EventBusiness;

public class UsePreparedReport(ILogger<UsePreparedReport> logger) 
    : IEventDriver<ReportIsHereEvent>
{
    private readonly ILogger<UsePreparedReport> _logger = logger;

    public async Task Execute(ReportIsHereEvent appEvent)
    {
        _logger.LogInformation("Document Accepted, Trace Id : {TraceId}, Document Id : {CreatedReportId}"
            , appEvent.TraceId, appEvent.CreatedReportId);

        // Dokümanı Local Storage'dan oku
        // E-posta ile gönder
        // Raporun hazırlandığına dair bir bilgilendirme olayı(ReportProcessCompleted) hazırlayıp fırlat
    }
}
