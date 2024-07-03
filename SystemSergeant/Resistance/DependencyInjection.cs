using Microsoft.AspNetCore.Builder;

namespace Resistance;

public static class DependencyInjection
{
    public static IApplicationBuilder AddResistance(this IApplicationBuilder app, Options options)
    {
        if (!options.NetworkFailureIsActive)
            app.UseMiddleware<NetworkFailureBehavior>(options.NetworkFailureProbability);

        return app;
    }
}