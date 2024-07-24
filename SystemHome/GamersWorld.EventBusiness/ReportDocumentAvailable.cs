using System.Net.Http.Json;
using GamersWorld.Domain.Enums;
using GamersWorld.Domain.Responses;
using Microsoft.Extensions.Logging;
using GamersWorld.Domain.Requests;
using GamersWorld.Domain.Constants;
using GamersWorld.Application.Contracts.Events;
using GamersWorld.Application.Contracts.Document;
using Microsoft.Extensions.DependencyInjection;

namespace GamersWorld.EventBusiness;

public class ReportDocumentAvailable(
    ILogger<ReportDocumentAvailable> logger
    , IHttpClientFactory httpClientFactory
    , IServiceProvider serviceProvider)
    : IEventDriver<ReportReadyEvent>
{
    private readonly ILogger<ReportDocumentAvailable> _logger = logger;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task Execute(ReportReadyEvent appEvent)
    {
        var client = _httpClientFactory.CreateClient(Names.KahinGateway);
        _logger.LogInformation("{TraceId}, Ref Doc: {CreatedReportId}", appEvent.TraceId, appEvent.CreatedReportId);

        var payload = new
        {
            DocumentId = appEvent.CreatedReportId
        };
        var response = await client.PostAsJsonAsync("/getReport", payload);
        _logger.LogInformation("GetReport call status code is {StatusCode}", response.StatusCode);

        // QUESTION: Doküman çekilen servise ulaşılamadı. Akış nasıl devam etmeli?
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Document fetching error");
            return;
        }

        var getReportResponse = await response.Content.ReadFromJsonAsync<GetReportResponse>();

        if (getReportResponse is { StatusCode: StatusCode.ReportReady })
        {
            _logger.LogWarning("{DocumentId} is ready and fetching...", getReportResponse.DocumentId);
            var content = getReportResponse.Document;
            // Başka bir Business enstrüman kullanılarak yazma işlemi gerçekleştirilir
            var docContent = new ReportSaveRequest
            {
                TraceId = appEvent.TraceId,
                Title = appEvent.Title,
                Expression = appEvent.Expression,
                EmployeeId = appEvent.EmployeeId,
                DocumentId = getReportResponse.DocumentId,
                Content = content,
                InsertTime = DateTime.Now,
                ExpireTime = DateTime.Now.AddMinutes(appEvent.ExpireTime.TotalMinutes),
            };

            var writeOperator = _serviceProvider.GetRequiredKeyedService<IDocumentWriter>(Names.DbWriteService);

            var saveResponse = await writeOperator.SaveAsync(docContent);
            _logger.LogInformation("Save response is {StatusCode} and message is {Message}"
            , saveResponse.StatusCode, saveResponse.Message);
        }
    }
}
