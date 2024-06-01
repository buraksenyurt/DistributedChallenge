using System.Text;
using System.Text.Json;
using GamersWorld.Common.Messages.Requests;
using GamersWorld.Common.Messages.Responses;

public class MessengerServiceClient
{

    private readonly HttpClient _httpClient;
    private readonly ILogger<MessengerServiceClient> _logger;
    public MessengerServiceClient(HttpClient httpClient, ILogger<MessengerServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<BusinessResponse> SendNewReportRequestAsync(NewReportRequest request)
    {
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("http://localhost:5234/", content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BusinessResponse>(responseContent);
        }
        else
        {
            _logger.LogError("Rapor talebi gönderiminde hata: {}", response.ReasonPhrase);
            throw new HttpRequestException($"Rapor talebi gönderiminde hata: {response.ReasonPhrase}");
        }
    }
}