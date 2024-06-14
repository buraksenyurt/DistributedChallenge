using System.Net.Http.Json;
using Kahin.Common.Constants;
using Kahin.Common.Requests;
using Kahin.Common.Responses;
using Microsoft.Extensions.Logging;

namespace Kahin.Common.Validation
{
    public class ValidatorClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ValidatorClient> _logger;

        public ValidatorClient(IHttpClientFactory httpClientFactory, ILogger<ValidatorClient> logger)
        {
            _httpClient = httpClientFactory.CreateClient(Names.EvalApi);
            _logger = logger;
            _logger.LogWarning("{ClientAddress}", _httpClient.BaseAddress);
        }

        public async Task<bool> ValidateExpression(CreateReportRequest request)
        {
            _logger.LogInformation("Audit function is working!");
            var payload = new
            {
                Source = Names.SourceDomain,
                request.Expression
            };
            var evalResponse = await _httpClient.PostAsJsonAsync("", payload);
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
}
