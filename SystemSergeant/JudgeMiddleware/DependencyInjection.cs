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
    public static IApplicationBuilder AddJudgeMiddleware(this IApplicationBuilder app, MetricOptions metricOptions)
    {
        if (!metricOptions.DeactivatePerformanceBehavior)
            app.UseMiddleware<PerformanceBehavior>(metricOptions);

        if (!metricOptions.DeactivateInputOutputBehavior)
            app.UseMiddleware<InputOutputBehavior>();

        return app;
    }
}