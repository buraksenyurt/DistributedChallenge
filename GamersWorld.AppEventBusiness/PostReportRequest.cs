using GamersWorld.AppEvents;
using GamersWorld.SDK;
using GamersWorld.SDK.Enums;
using GamersWorld.SDK.Messages;
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
    public PostReportRequest(ILogger<PostReportRequest> logger)
    {
        _logger = logger;
    }
    public async Task<BusinessResponse> Execute(ReportRequestedEvent appEvent)
    {
        _logger.LogInformation("{}, {}, {}", appEvent.TraceId, appEvent.Title, appEvent.Expression);
        //TODO@buraksenyurt Must implement App Service Post request

        // Reporting App Service'e bir POST talebi yapılır
        return new BusinessResponse
        {
            Message = "Rapor talebi gönderildi",
            StatusCode = StatusCode.Success,
        };
    }
}
