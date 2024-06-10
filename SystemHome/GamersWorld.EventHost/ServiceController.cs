using Microsoft.Extensions.Logging;

namespace GamersWorld.EventHost;

public static class ServiceController
{
    public static async Task<bool> IsReportingServiceAlive(HttpClient client, ILogger logger)
    {
        try
        {
            var response = await client.GetAsync("/health");

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Hedef servis için sağlık kontrolü başarısız !!!");

            return false;
        }
    }

}