using System.Net.Http.Json;
using GamersWorld.Domain.Enums;
using GamersWorld.Domain.Responses;
using Microsoft.Extensions.Logging;
using GamersWorld.Domain.Constants;
using GamersWorld.Application.Contracts.Events;

namespace GamersWorld.EventBusiness;

public class NewReportRequest(ILogger<NewReportRequest> logger, IHttpClientFactory httpClientFactory)
    : IEventDriver<ReportRequestedEvent>
{
    private readonly ILogger<NewReportRequest> _logger = logger;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public async Task Execute(ReportRequestedEvent appEvent)
    {
        var client = _httpClientFactory.CreateClient(Names.KahinGateway);
        _logger.LogInformation("{TraceId}, {Title}, {Expression}", appEvent.TraceId, appEvent.Title, appEvent.Expression);

        var payload = new
        {
            appEvent.TraceId,
            appEvent.Title,
            appEvent.Expression,
            appEvent.EmployeeId,
            ExpireTime = TimeSpan.FromMinutes((int)appEvent.Lifetime)
        };

        _logger.LogInformation("Service Uri : {ServiceUri}", client.BaseAddress);
        var response = await client.PostAsJsonAsync("/", payload);

        if (response.IsSuccessStatusCode)
        {
            var createReportResponse = await response.Content.ReadFromJsonAsync<CreateReportResponse>();

            if (createReportResponse is { Status: StatusCode.Success })
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
