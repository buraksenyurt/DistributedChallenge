using System.Net.Http.Json;
using Kahin.Common.Requests;
using Kahin.Common.Responses;

namespace Kahin.Common.Validation
{
    public class ValidatorClient
    {
        private readonly HttpClient _httpClient;

        public ValidatorClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("EvalApi");
        }

        public async Task<bool> ValidateExpression(CreateReportRequest request)
        {
            var payload = new
            {
                Source = "KahinDomain",
                request.Expression
            };
            var evalResponse = await _httpClient.PostAsJsonAsync("/", payload);
            if (evalResponse.IsSuccessStatusCode)
            {
                var evalResult = await evalResponse.Content.ReadFromJsonAsync<ExpressionCheckResponse>();
                return evalResult != null && evalResult.IsValid;
            }
            return false;
        }
    }
}
