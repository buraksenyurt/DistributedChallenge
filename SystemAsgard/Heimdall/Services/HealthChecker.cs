using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Heimdall.Services;

public class HealthChecker(Uri serviceAddress) : IHealthCheck
{
    private readonly Uri _serviceAddress = serviceAddress;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        using var client = new HttpClient();
        var response = await client.GetAsync(_serviceAddress, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return HealthCheckResult.Healthy();
        }

        return HealthCheckResult.Unhealthy();
    }
}