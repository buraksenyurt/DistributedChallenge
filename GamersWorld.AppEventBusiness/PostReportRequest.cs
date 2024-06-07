using System.Net.Http.Json;
using GamersWorld.AppEvents;
using GamersWorld.Common.Enums;
using GamersWorld.Common.Messages.Responses;
using GamersWorld.SDK;
using Microsoft.Extensions.Logging;

namespace GamersWorld.AppEventBusiness;

/*
    Kullanıcı form aracılığı ile bir rapor talebinde bulunduğunda Event Trigger Service
    yeni bir ReportRequestedEvent nesnesi hazırlar ve bunu kuyruğa gönderir.

    Kuyruğu dinleyen taraf bu event gerçekleştiğinde 
    aşağıdaki nesnenin Execute metodunu icra edip buradaki işlemleri yapmalıdır.
*/
public class PostReportRequest
    : IEventDriver<ReportRequestedEvent>
{
    private readonly ILogger<PostReportRequest> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    public PostReportRequest(ILogger<PostReportRequest> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }
    public async Task<BusinessResponse> Execute(ReportRequestedEvent appEvent)
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
                return new BusinessResponse
                {
                    Message = $"Rapor talebi iletildi. DocumentId: {createReportResponse.DocumentId}",
                    StatusCode = StatusCode.Success,
                };
            }
            else if (createReportResponse != null)
            {
                _logger.LogError("Rapor talebi başarısız oldu");
                return new BusinessResponse
                {
                    Message = !string.IsNullOrEmpty(createReportResponse.Explanation) ? createReportResponse.Explanation : "Eksik açıklama",
                    StatusCode = StatusCode.Fail,
                };
            }
        }

        return new BusinessResponse
        {
            Message = "Rapor talebi gönderimi başarısız",
            StatusCode = StatusCode.Fail,
        };
    }
}
