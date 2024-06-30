namespace GamersWorld.WebApp.Services;

using GamersWorld.Domain.Constants;
using GamersWorld.Domain.Data;
using GamersWorld.Domain.Requests;
using GamersWorld.Domain.Responses;
using SecretsAgent;

public class MessengerServiceClient(HttpClient httpClient, ISecretStoreService secretStoreService, ILogger<MessengerServiceClient> logger)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<MessengerServiceClient> _logger = logger;
    private readonly ISecretStoreService _secretStoreService = secretStoreService;

    public async Task<IEnumerable<ReportDocument>> GetReportDocumentsByEmployeeAsync(GetReportsByEmployeeRequest request)
    {
        var messengerApiAddress = await _secretStoreService.GetSecretAsync(SecretName.MessengerApiAddress);
        var url = $"http://{messengerApiAddress}?EmployeeId={request.EmployeeId}";
        var response = await _httpClient.GetFromJsonAsync<IEnumerable<ReportDocument>>(url);
        return response;
    }

    public async Task<BusinessResponse> SendNewReportRequestAsync(NewReportRequest request)
    {
        var messengerApiAddress = await _secretStoreService.GetSecretAsync(SecretName.MessengerApiAddress);
        var response = await _httpClient.PostAsJsonAsync($"http://{messengerApiAddress}/", request);

        if (!response.IsSuccessStatusCode)
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
                StatusCode = Domain.Enums.StatusCode.Fail,
                Message = "Not OK(200)"
            };
        }
        else
        {
            var result = await response.Content.ReadFromJsonAsync<BusinessResponse>();
            if (result == null)
            {
                return new BusinessResponse
                {
                    StatusCode = Domain.Enums.StatusCode.Fail,
                    Message = "Not OK(200)"
                };
            }
            return result;
        }
    }
}
