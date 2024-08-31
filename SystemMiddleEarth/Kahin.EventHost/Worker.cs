using Kahin.Common.Constants;
using Kahin.Common.Enums;
using Kahin.Common.Requests;
using Kahin.Common.Services;
using Kahin.MQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kahin.EventHost;

public class Worker(
          IRedisService redisService,
          HomeGatewayServiceClient httpGatewayClient,
          ILogger<Worker> logger) : BackgroundService
{
    private readonly IRedisService _redisService = redisService;
    private readonly HomeGatewayServiceClient _httpGatewayClient = httpGatewayClient;
    private readonly ILogger<Worker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            var eventData = await _redisService.Pop(Names.EventStream);
            if (eventData != null)
            {
                switch (eventData.EventType)
                {
                    case EventType.ReportRequested:
                        _logger.LogWarning("New Report Requested. {EventData}", eventData);
                        await PrepareReportAsync(eventData, stoppingToken);
                        break;
                    case EventType.ReportReady:
                        var payload = new ReportStatusRequest
                        {
                            TraceId = eventData.TraceId,
                            EmployeeId = eventData.EmployeeId,
                            ReportTitle = eventData.ReportTitle,
                            Expression = eventData.Expression,
                            StatusCode = (int)StatusCode.ReportReady,
                            ExpireTime = eventData.ReportExpireTime,
                            StatusMessage = $"Report is ready and live for {eventData.ReportExpireTime.TotalMinutes} minutes",
                            DocumentId = eventData.DocumentId.ToString(),
                            Detail = ""
                        };
                        var response = await _httpGatewayClient.SendReportStatusAsync(payload);
                        _logger.LogInformation("Home Gateway API Response: {Response}", response);
                        break;
                    case EventType.AuditFail:
                        var auditFail = new ReportStatusRequest
                        {
                            TraceId = eventData.TraceId,
                            EmployeeId = eventData.EmployeeId,
                            ReportTitle = eventData.ReportTitle,
                            StatusCode = (int)StatusCode.InvalidExpression,
                            StatusMessage = "Audit validation error!",
                            DocumentId = eventData.DocumentId.ToString(),
                            Detail = ""
                        };
                        var _ = await _httpGatewayClient.SendReportStatusAsync(auditFail);
                        _logger.LogWarning("Audit validation failed for expression");
                        break;
                    case EventType.Error:
                        _logger.LogError("Error on Redis event streaming. {EventData}", eventData);
                        break;
                }
            }

            await Task.Delay(TimeCop.SleepDuration, stoppingToken);
        }
    }

    public async Task PrepareReportAsync(RedisPayload payload, CancellationToken cancellationToken)
    {
        if (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(TimeCop.WaitFactor * TimeCop.OneMilisecond, cancellationToken); // Sembolik olarak bir gecikme söz konusu
        }

        payload.EventType = EventType.ReportReady;
        await redisService.AddReportPayloadAsync(Names.EventStream, payload, TimeSpan.FromMinutes(TimeCop.SixtyMinutes));
    }
}