using Microsoft.AspNetCore.Builder;
using Resistance.Configuration;
using Resistance.Latency;
using Resistance.NetworkFailure;
using Resistance.Outage;
using Resistance.ResourceRace;

namespace Resistance;

public static class DependencyInjection
{
    public static IApplicationBuilder UseResistance(this IApplicationBuilder app, ResistanceOptions options)
    {
        app.UseMiddleware<NetworkFailureBehavior>(options.NetworkFailureProbability);
        app.UseMiddleware<LatencyBehavior>(options.LatencyPeriod);
        app.UseMiddleware<ResourceRaceBehavior>(options.ResourceRaceUpperLimit);
        app.UseMiddleware<OutageBehavior>(options.OutagePeriod);

        return app;
    }
}