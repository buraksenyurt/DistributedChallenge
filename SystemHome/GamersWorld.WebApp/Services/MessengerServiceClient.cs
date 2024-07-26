namespace GamersWorld.WebApp.Services;

using GamersWorld.Domain.Entity;
using GamersWorld.Domain.Dtos;
using GamersWorld.Domain.Requests;
using GamersWorld.Domain.Responses;
using System.Text;
using System.Text.Json;

public class MessengerServiceClient(HttpClient httpClient, ILogger<MessengerServiceClient> logger)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<MessengerServiceClient> _logger = logger;

    public async Task<IEnumerable<Report>> GetReportDocumentsByEmployeeAsync(GetReportsByEmployeeRequest request)
    {
        var response = await _httpClient.GetFromJsonAsync<IEnumerable<Report>>($"/api/documents/employee/{request.EmployeeId}");
        if (response == null)
        {
            _logger.LogWarning("There are no reports for {EmployeeId}", request.EmployeeId);
            return [];
        }
        return response;
    }

    public async Task<DocumentContentDto?> GetReportDocumentByIdAsync(string documentId)
    {
        var response = await _httpClient.GetFromJsonAsync<DocumentContentDto>($"/api/documents/{documentId}");
        if (response == null || response.Base64Content == null)
        {
            _logger.LogWarning("Requested {DocumentId} is null", documentId);
            return null;
        }
        return new DocumentContentDto
        {
            Base64Content = response.Base64Content,
            ContentSize = response.Base64Content.Length
        };
    }

    public async Task<BusinessResponse> SendNewReportRequestAsync(NewReportRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/documents/", request);

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
                Status = Domain.Enums.Status.Fail,
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
                    Status = Domain.Enums.Status.Fail,
                    Message = "Not OK(200)"
                };
            }
            return result;
        }
    }

    public async Task<BusinessResponse> DeleteDocumentByIdAsync(DeleteReportRequest request)
    {
        var requestUri = $"api/documents/{request.DocumentId}";
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, requestUri)
        {
            Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
        };

        var deleteResponse = await _httpClient.SendAsync(httpRequestMessage);
        if (!deleteResponse.IsSuccessStatusCode)
        {
            return new BusinessResponse
            {
                Status = Domain.Enums.Status.Fail,
                Message = "Fail on document delete"
            };
        }
        if (deleteResponse.StatusCode == System.Net.HttpStatusCode.Accepted)
        {
            _logger.LogInformation("{DocumentId} delete request has been sent.", request.DocumentId);
            return new BusinessResponse
            {
                Status = Domain.Enums.Status.DeleteRequestAccepted,
                Message = "Document delete request has been sent."
            };
        }
        return new BusinessResponse
        {
            Status = Domain.Enums.Status.Fail,
            Message = "Document deletion unsuccesfull!"
        };
    }

    public async Task<BusinessResponse> ArchiveDocumentByIdAsync(ArchiveReportRequest request)
    {
        var archiveResponse = await _httpClient.PostAsJsonAsync($"/api/documents/archive", request);
        if (!archiveResponse.IsSuccessStatusCode)
        {
            return new BusinessResponse
            {
                Status = Domain.Enums.Status.Fail,
                Message = "Fail on document archive"
            };
        }
        if (archiveResponse.StatusCode == System.Net.HttpStatusCode.OK)
        {
            _logger.LogInformation("{DocumentId} archived", request.DocumentId);
            return new BusinessResponse
            {
                Status = Domain.Enums.Status.Success,
                Message = "Document succesfully archived!"
            };
        }
        return new BusinessResponse
        {
            Status = Domain.Enums.Status.Fail,
            Message = "Document archive process unsuccesfull!"
        };
    }
}
