namespace GamersWorld.WebApp.Utility;

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
        var response = await _httpClient.PostAsJsonAsync("http://localhost:5234/", request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<BusinessResponse>();
        }
        else
        {
            _logger.LogError("Rapor talebi gönderiminde hata: {ReasonePhrase}", response.ReasonPhrase);
            throw new HttpRequestException($"Rapor talebi gönderiminde hata: {response.ReasonPhrase}");
        }
    }
}
