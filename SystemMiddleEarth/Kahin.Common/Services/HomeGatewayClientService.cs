using System.Text;
using System.Text.Json;
using Kahin.Common.Requests;
using Microsoft.Extensions.Logging;

namespace Kahin.Common.Services;

public class HomeGatewayClientService(HttpClient httpClient, ILogger<HomeGatewayClientService> logger, ISecretStoreService secretStoreService) : IHomeGatewayClientService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<HomeGatewayClientService> _logger = logger;
    private readonly ISecretStoreService _secretStoreService = secretStoreService;

    public async Task<string> Post(ReportStatusRequest request)
    {
        var payload = JsonSerializer.Serialize(request);
        var url = await _secretStoreService.GetSecretAsync("HomeGatewayApiAddress");
        _logger.LogInformation("Payload for sending {Payload}", payload);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"http://{url}", content);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}