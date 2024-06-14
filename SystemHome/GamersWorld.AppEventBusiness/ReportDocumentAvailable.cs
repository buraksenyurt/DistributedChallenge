using System.Net.Http.Json;
using GamersWorld.AppEvents;
using GamersWorld.Common.Enums;
using GamersWorld.Common.Messages.Responses;
using GamersWorld.Common.Responses;
using GamersWorld.SDK;
using Microsoft.Extensions.Logging;

namespace GamersWorld.AppEventBusiness;

/*
    Reporting App Service tarafı rapor hazır olduğunda External Reader Service'i tetikler
    ve raporun hazır olduğunu HTTP Post çağrısı ile bildirir.

    External Reader Service bunun üzerine ReportReadyEvent hazırlar ve kuyruğa bırakır.

    Kuyruk dinleyicisi bu event'i yakalarsa aşağıdaki sınıfa ait nesne örneğini kullanır.
    Execute içerisindeki işlemler yapılır.
*/
public class ReportDocumentAvailable(
    ILogger<ReportDocumentAvailable> logger
    , IHttpClientFactory httpClientFactory)
    : IEventDriver<ReportReadyEvent>
{
    private readonly ILogger<ReportDocumentAvailable> _logger = logger;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public async Task<BusinessResponse> Execute(ReportReadyEvent appEvent)
    {
        var client = _httpClientFactory.CreateClient("KahinGateway");
        _logger.LogInformation("{TraceId}, Ref Doc: {CreatedReportId}", appEvent.TraceId, appEvent.CreatedReportId);

        var payload = new
        {
            DocumentId = appEvent.CreatedReportId
        };
        var response = await client.PostAsJsonAsync("/getReport", payload);
        _logger.LogInformation("GetReport call status code is {StatusCode}", response.StatusCode);
        if (response.IsSuccessStatusCode)
        {
            var getReportResponse = await response.Content.ReadFromJsonAsync<GetReportResponse>();

            // _logger.LogInformation("GetReport call contents\n\t{StatusCode}\n\t{DocumentId}"
            // , getReportResponse.StatusCode
            // , getReportResponse.DocumentId);

            if (getReportResponse != null && getReportResponse.StatusCode == StatusCode.ReportReady)
            {
                _logger.LogWarning("{DocumentId} is ready and fetching...", getReportResponse.DocumentId);
                var content = getReportResponse.Document;
                // Şimdilik deneysel olarak bir dosya yazdırma işlemi söz konusu.
                // Burada gelen byte içeriğini yazma işlemi izole edip DI servislerinden gelen bir bileşen ile ele alınabilir.
                await File.WriteAllBytesAsync(Path.Combine(Environment.CurrentDirectory, $"{getReportResponse.DocumentId}.csv"), content);
            }
        }

        // ReportIsHere olayını hazırla ve kuyruğa bırak
        return new BusinessResponse
        {
            StatusCode = StatusCode.ReportReady,
            Message = "Report is available."
        };
    }
}
