using System.Text;
using System.Text.Json;
using Kahin.Common.Requests;
using Microsoft.Extensions.Logging;

namespace Kahin.Common.Services;

public class HomeGatewayClientService : IHomeGatewayClientService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HomeGatewayClientService> _logger;
    public HomeGatewayClientService(HttpClient httpClient, ILogger<HomeGatewayClientService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    public async Task<string> Post(string url, ReportStatusRequest request)
    {
        var payload = JsonSerializer.Serialize(request);
        _logger.LogInformation("Payload for sending {Payload}", payload);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}