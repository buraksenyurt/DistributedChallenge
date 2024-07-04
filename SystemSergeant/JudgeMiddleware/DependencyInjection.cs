using Microsoft.AspNetCore.Builder;

namespace JudgeMiddleware;

public static class DependencyInjection
{
    public static IApplicationBuilder AddJudgeMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<PerformanceBehavior>();
        app.UseMiddleware<InputOutputBehavior>();

        return app;
    }
    public static IApplicationBuilder AddJudgeMiddleware(this IApplicationBuilder app, Options options)
    {
        if (!options.DeactivatePerformanceBehavior)
            app.UseMiddleware<PerformanceBehavior>(options);

        if (!options.DeactivateInputOutputBehavior)
            app.UseMiddleware<InputOutputBehavior>(options);

        return app;
    }
}