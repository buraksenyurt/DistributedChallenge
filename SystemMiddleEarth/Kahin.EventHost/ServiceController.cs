using Microsoft.Extensions.Logging;

namespace Kahin.EventHost;

public static class ServiceController
{
    public static async Task<bool> IsReportingServiceAlive(HttpClient client, ILogger logger)
    {
        logger.LogInformation("Health check for {Address}", client.BaseAddress);
        var response = await client.GetAsync("/health");
        return response.IsSuccessStatusCode;
    }
}