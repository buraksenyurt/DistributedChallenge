using System.Net.Http.Json;
using Kahin.Common.Constants;
using Kahin.Common.Requests;
using Kahin.Common.Responses;
using Microsoft.Extensions.Logging;

namespace Kahin.Common.Validation;

public class ValidatorClient(HttpClient httpClient, ILogger<ValidatorClient> logger)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<ValidatorClient> _logger = logger;

    public async Task<bool> ValidateExpression(CreateReportRequest request)
    {
        _logger.LogInformation("Audit function is working!");
        var payload = new
        {
            Source = Names.SourceDomain,
            request.Expression
        };
        var evalResponse = await _httpClient.PostAsJsonAsync("/api", payload);
        _logger.LogInformation("Response; {Response}", evalResponse);
        if (evalResponse.IsSuccessStatusCode)
        {
            var evalResult = await evalResponse.Content.ReadFromJsonAsync<ExpressionCheckResponse>();
            _logger.LogInformation("Eval Result; {Response}", evalResult);

            return evalResult != null && evalResult.IsValid;
        }

        return false;
    }
}
