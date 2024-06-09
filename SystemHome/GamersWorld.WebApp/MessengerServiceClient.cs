namespace GamersWorld.WebApp.Utility;

using GamersWorld.Common.Messages.Requests;
using GamersWorld.Common.Messages.Responses;

public class MessengerServiceClient(HttpClient httpClient, ILogger<MessengerServiceClient> logger)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<MessengerServiceClient> _logger = logger;

    public async Task<BusinessResponse> SendNewReportRequestAsync(NewReportRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("http://localhost:5234/", request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<BusinessResponse>();
        }
        else
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<BusinessResponse>();
            if (errorResponse != null && errorResponse.ValidationErrors != null)
            {
                _logger.LogError("There are validation errors. {ValidationErrors}",
                    string.Join("; ", errorResponse.ValidationErrors.Select(e => $"{e.Key}: {string.Join(", ", e.Value)}")));
            }
            else
            {
                _logger.LogError("There are validation errors. Reason is '{ReasonPhrase}'", response.ReasonPhrase);
            }

            return errorResponse ?? new BusinessResponse
            {
                StatusCode = Common.Enums.StatusCode.Fail,
                Message = "Not OK(200)"
            };
        }
    }
}
