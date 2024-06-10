using Kahin.Common.Enums;
using Kahin.Common.Requests;
using Kahin.Common.Services;
using Kahin.MQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kahin.EventHost;

public class Worker(
    IRedisService redisService
        , IHomeGatewayClientService httpGatewayClient
        , IConfiguration configuration, ILogger<Worker> logger)
        : BackgroundService
{
    private readonly IRedisService _redisService = redisService;
    private readonly IHomeGatewayClientService _httpGatewayClient = httpGatewayClient;
    private readonly ILogger<Worker> _logger = logger;
    private readonly string _gatewayProxyHostAddress = configuration["HomeGatewayApi:Address"] ?? "http://localhost:5102";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            var eventData = await _redisService.Pop("reportStream");
            if (eventData != null)
            {
                _logger.LogInformation("Received eventData: {EventData}", eventData);

                var payload = new ReportStatusRequest
                {
                    TraceId = eventData.TraceId,
                    StatusCode = (int)StatusCode.ReportReady,
                    StatusMessage = "Report is ready and live for 60 minutes",
                    DocumentId = eventData.DocumentId.ToString(),
                    Detail = ""
                };
                var response = await _httpGatewayClient.Post(_gatewayProxyHostAddress, payload);                
                _logger.LogInformation("Home Gateway API Response: {Response}", response);
            }

            await Task.Delay(10000, stoppingToken);
        }
    }
}