using Microsoft.Extensions.Logging;

namespace GamersWorld.EventHost;

public static class ServiceController
{
    public static async Task<bool> IsReportingServiceAlive(HttpClient client, ILogger logger)
    {
        try
        {
            logger.LogInformation("Health check for {Address}", client.BaseAddress);
            var response = await client.GetAsync("/health");

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Healt check unsuccedded");

            return false;
        }
    }

}