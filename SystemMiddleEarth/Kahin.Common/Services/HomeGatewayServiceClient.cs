using System.Net.Http.Json;
using Kahin.Common.Requests;
using Microsoft.Extensions.Logging;

namespace Kahin.Common.Services;

public class HomeGatewayServiceClient(
    HttpClient httpClient
    , ILogger<HomeGatewayServiceClient> logger)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<HomeGatewayServiceClient> _logger = logger;

    public async Task<string> SendReportStatusAsync(ReportStatusRequest request)
    {
        _logger.LogInformation("'{ReportTitle}'-{DocumentId} is sending", request.ReportTitle, request.DocumentId);

        var response = await _httpClient.PostAsJsonAsync($"/", request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}