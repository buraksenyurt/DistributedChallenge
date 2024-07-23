using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Heimdall.Services;

public class HealthChecker(IHttpClientFactory httpClientFactory, string clientName) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient(clientName);
        var response = await client.GetAsync("/health", cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return HealthCheckResult.Healthy();
        }

        return HealthCheckResult.Unhealthy();
    }
}
