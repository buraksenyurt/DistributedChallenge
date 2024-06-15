using System.Net.Http.Json;
using GamersWorld.Events;
using GamersWorld.Business.Contracts;
using GamersWorld.Common.Enums;
using GamersWorld.Common.Responses;
using GamersWorld.SDK;
using Microsoft.Extensions.Logging;
using GamersWorld.Common.Requests;

namespace GamersWorld.EventBusiness;

/*
    Reporting App Service tarafı rapor hazır olduğunda External Reader Service'i tetikler
    ve raporun hazır olduğunu HTTP Post çağrısı ile bildirir.

    External Reader Service bunun üzerine ReportReadyEvent hazırlar ve kuyruğa bırakır.

    Kuyruk dinleyicisi bu event'i yakalarsa aşağıdaki sınıfa ait nesne örneğini kullanır.
    Execute içerisindeki işlemler yapılır.
*/
public class ReportDocumentAvailable(
    ILogger<ReportDocumentAvailable> logger
    , IHttpClientFactory httpClientFactory
    , IDocumentSaver documentSaver)
    : IEventDriver<ReportReadyEvent>
{
    private readonly ILogger<ReportDocumentAvailable> _logger = logger;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IDocumentSaver _documentSaver = documentSaver;

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

        if (!response.IsSuccessStatusCode)
        {
            return new BusinessResponse
            {
                StatusCode = StatusCode.Fail,
                Message = "Save document failed!"
            };
        }

        var getReportResponse = await response.Content.ReadFromJsonAsync<GetReportResponse>();

        // _logger.LogInformation("GetReport call contents\n\t{StatusCode}\n\t{DocumentId}"
        // , getReportResponse.StatusCode
        // , getReportResponse.DocumentId);

        if (getReportResponse != null && getReportResponse.StatusCode == StatusCode.ReportReady)
        {
            _logger.LogWarning("{DocumentId} is ready and fetching...", getReportResponse.DocumentId);
            var content = getReportResponse.Document;
            // Başka bir Business enstrüman kullanılarak yazma işlemi gerçekleştirilir
            var docContent = new DocumentSaveRequest
            {
                TraceId = appEvent.TraceId,
                DocumentId = getReportResponse.DocumentId,
                Content = content,
            };
            var length = await _documentSaver.SaveTo(docContent);
            if (length == 0)
            {
                return new BusinessResponse
                {
                    StatusCode = StatusCode.Fail,
                    Message = "Report document has not been saved."
                };
            }
        }

        return new BusinessResponse
        {
            StatusCode = StatusCode.DocumentSaved,
            Message = "Report document has been saved."
        };
    }
}
