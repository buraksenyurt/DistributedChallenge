using System.Net.Http;
using System.Text;
using System.Text.Json;
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
    private readonly HttpClient _httpClient;
    public PostReportRequest(ILogger<PostReportRequest> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }
    public async Task<BusinessResponse> Execute(ReportRequestedEvent appEvent)
    {
        _logger.LogInformation("{}, {}, {}", appEvent.TraceId, appEvent.Title, appEvent.Expression);

        var payload = new
        {
            appEvent.TraceId,
            appEvent.Title,
            appEvent.Expression
        };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/", content);
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var createReportResponse = JsonSerializer.Deserialize<CreateReportResponse>(responseContent);

            if (createReportResponse != null && createReportResponse.Status == StatusCode.Success)
            {
                _logger.LogInformation("{Response}", createReportResponse);
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
