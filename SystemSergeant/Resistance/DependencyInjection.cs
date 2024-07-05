using Microsoft.AspNetCore.Builder;

namespace Resistance;

public static class DependencyInjection
{
    public static IApplicationBuilder UseResistance(this IApplicationBuilder app, ResistanceOptions options)
    {
        if (options.NetworkFailureIsActive)
            app.UseMiddleware<NetworkFailureBehavior>(options.NetworkFailureProbability);

        if (options.LatencyIsActive)
            app.UseMiddleware<LatencyBehavior>(options.LatencyPeriod);

        if (options.ResourceRaceIsActive)
            app.UseMiddleware<ResourceRaceBehavior>(options.ResourceRaceUpperLimit);

        if (options.OutageIsActive)
            app.UseMiddleware<OutageBehavior>(options.OutagePeriod);

        return app;
    }
}