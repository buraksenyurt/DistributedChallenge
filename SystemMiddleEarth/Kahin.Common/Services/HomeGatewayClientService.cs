using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Kahin.Common.Constants;
using Kahin.Common.Requests;
using Microsoft.Extensions.Logging;

namespace Kahin.Common.Services;

public class HomeGatewayClientService(
    HttpClient httpClient
    , ILogger<HomeGatewayClientService> logger
    , ISecretStoreService secretStoreService)
    : IHomeGatewayClientService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<HomeGatewayClientService> _logger = logger;
    private readonly ISecretStoreService _secretStoreService = secretStoreService;

    public async Task<string> Post(ReportStatusRequest request)
    {
        var url = await _secretStoreService.GetSecretAsync(SecretName.HomeGatewayApiAddress);
        // Enable this with JsonSerializer.Serialize if you want to see the payload
        // _logger.LogInformation("Payload for sending {Payload}", payload);
        
        var response = await _httpClient.PostAsJsonAsync($"http://{url}", request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}