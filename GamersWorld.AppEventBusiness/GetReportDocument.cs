using GamersWorld.AppEvents;
using GamersWorld.SDK;
using GamersWorld.SDK.Messages;
using Microsoft.Extensions.Logging;

namespace GamersWorld.AppEventBusiness;

/*
    Reporting App Service tarafı rapor hazır olduğunda External Reader Service'i tetikler
    ve raporun hazır olduğunu HTTP Post çağrısı ile bildirir.

    External Reader Service bunun üzerine ReportReadyEvent hazırlar ve kuyruğa bırakır.

    Kuyruk dinleyicisi bu event'i yakalarsa aşağıdaki sınıfa ait nesne örneğini kullanır.
    Execute içerisindeki işlemler yapılır.
*/
public class GetReportDocument
    : IEventDriver<ReportReadyEvent>
{
    private readonly ILogger<GetReportDocument> _logger;
    public GetReportDocument(ILogger<GetReportDocument> logger)
    {
        _logger = logger;
    }
    public async Task<BusinessResponse> Execute(ReportReadyEvent appEvent)
    {
        //TODO@buraksenyurt Must implement Get Report Document
        _logger.LogInformation($"{appEvent.TraceId}, Ref Doc: {appEvent.CreatedReportId}");
        // appEvent üstünden doküman nosunu al
        // Reporting File Service'i Get ile çağır ve dokümanı çek
        // Dokümanı Local Storage'a(kuvvetle muhtemel bir tabloya) geçici olarak kaydet
        // ReportIsHere olayını hazırla ve kuyruğa bırak
        throw new NotImplementedException();
    }
}
