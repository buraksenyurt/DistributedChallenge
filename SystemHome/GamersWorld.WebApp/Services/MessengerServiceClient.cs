namespace GamersWorld.WebApp.Services;

using GamersWorld.Domain.Data;
using GamersWorld.Domain.Dtos;
using GamersWorld.Domain.Requests;
using GamersWorld.Domain.Responses;

public class MessengerServiceClient(HttpClient httpClient, ILogger<MessengerServiceClient> logger)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<MessengerServiceClient> _logger = logger;

    public async Task<IEnumerable<Document>> GetReportDocumentsByEmployeeAsync(GetReportsByEmployeeRequest request)
    {
        var response = await _httpClient.GetFromJsonAsync<IEnumerable<Document>>($"?EmployeeId={request.EmployeeId}");
        if (response == null)
        {
            _logger.LogWarning("There are no reports for {EmployeeId}", request.EmployeeId);
            return [];
        }
        return response;
    }

    public async Task<DocumentContent?> GetReportDocumentByIdAsync(GetReportDocumentByIdRequest request)
    {
        var response = await _httpClient.GetFromJsonAsync<DocumentContent>($"/document?DocumentId={request.DocumentId}");
        if (response == null || response.Base64Content == null)
        {
            _logger.LogWarning("Requested {DocumentId} is null", request.DocumentId);
            return null;
        }
        return new DocumentContent
        {
            Base64Content = response.Base64Content,
            ContentSize = response.Base64Content.Length
        };
    }

    public async Task<BusinessResponse> SendNewReportRequestAsync(NewReportRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/", request);

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
