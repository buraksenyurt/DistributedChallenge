using System.Text;
using System.Text.Json;
using Kahin.Common.Requests;

namespace Kahin.Common.Services;

public class HomeGatewayClientService : IHomeGatewayClientService
{
    private readonly HttpClient _httpClient;
    public HomeGatewayClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<string> Post(string url, ReportStatusRequest request)
    {
        var payload = JsonSerializer.Serialize(request);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}