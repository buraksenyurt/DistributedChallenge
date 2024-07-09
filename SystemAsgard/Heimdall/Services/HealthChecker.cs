using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Heimdall.Services;

public class HealthChecker(IHttpClientFactory httpClientFactory, string clientName) : IHealthCheck
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly string _clientName = clientName;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient(_clientName);
        var response = await client.GetAsync("/health", cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return HealthCheckResult.Healthy();
        }

        return HealthCheckResult.Unhealthy();
    }
}
