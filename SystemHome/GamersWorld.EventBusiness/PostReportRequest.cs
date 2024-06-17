using System.Net.Http.Json;
using GamersWorld.Events;
using GamersWorld.Common.Enums;
using GamersWorld.Common.Responses;
using GamersWorld.SDK;
using Microsoft.Extensions.Logging;

namespace GamersWorld.EventBusiness;

public class PostReportRequest(ILogger<PostReportRequest> logger, IHttpClientFactory httpClientFactory)
    : IEventDriver<ReportRequestedEvent>
{
    private readonly ILogger<PostReportRequest> _logger = logger;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public async Task Execute(ReportRequestedEvent appEvent)
    {
        var client = _httpClientFactory.CreateClient("KahinGateway");
        _logger.LogInformation("{TraceId}, {Title}, {Expression}", appEvent.TraceId, appEvent.Title, appEvent.Expression);

        var payload = new
        {
            appEvent.TraceId,
            appEvent.Title,
            appEvent.Expression
        };

        _logger.LogInformation("Service Uri : {ServiceUri}", client.BaseAddress);
        var response = await client.PostAsJsonAsync("/", payload);

        if (response.IsSuccessStatusCode)
        {
            var createReportResponse = await response.Content.ReadFromJsonAsync<CreateReportResponse>();

            if (createReportResponse != null && createReportResponse.Status == StatusCode.Success)
            {
                _logger.LogInformation("Report request sent({Status}). {DocumentId}"
                    , createReportResponse.Status, createReportResponse.DocumentId);
                return;
            }
            else
            {
                // QUESTION: Rapor gönderimi başarısız ise buna karşılık başka bir business tetiklenmeli mi?
                _logger.LogError("Report request unsuccessful.");
            }
        }
        else
        {
            // QUESTION: Rapor gönderimi başarısız ise buna karşılık başka bir business tetiklenmeli mi?   
            _logger.LogError("Report request unsuccessful.");
        }
    }
}
