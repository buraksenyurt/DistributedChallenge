using GamersWorld.Events;
using GamersWorld.Common.Responses;
using GamersWorld.SDK;
using Microsoft.Extensions.Logging;

namespace GamersWorld.EventBusiness;

/*
    Rapor karşı sistemden diğer sistemin Local Storage'ına indiğinde
    ReportIsHereEvent hazırlanıp kuyruğa bırakılmış oluyor. 

    Bu olay yakalanırsa aşağıdaki nesne fonksiyonundaki işlemler yapılacak.
*/
public class UsePreparedReport(ILogger<UsePreparedReport> logger) : IEventDriver<ReportIsHereEvent>
{
    private readonly ILogger<UsePreparedReport> _logger = logger;

    public async Task<BusinessResponse> Execute(ReportIsHereEvent appEvent)
    {
        _logger.LogInformation("Document Accepted, Trace Id : {TraceId}, Document Id : {CreatedReportId}"
            , appEvent.TraceId, appEvent.CreatedReportId);

        // Dokümanı Local Storage'dan oku
        // E-posta ile gönder
        // Raporun hazırlandığına dair bir bilgilendirme olayı(ReportProcessCompleted) hazırlayıp fırlat

        return new BusinessResponse
        {
            StatusCode = Common.Enums.StatusCode.Success,
            Message = "Report document fetched and send succesfully"
        };
    }
}
