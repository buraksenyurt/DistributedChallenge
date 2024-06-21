using Microsoft.AspNetCore.Builder;

namespace JudgeMiddleware;

public static class DependencyInjection
{
    public static IApplicationBuilder AddJudgeMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<PerformanceBehavior>();

        return app;
    }
    public static IApplicationBuilder AddJudgeMiddleware(this IApplicationBuilder app, MetricOptions metricOptions)
    {
        app.UseMiddleware<PerformanceBehavior>(metricOptions);

        return app;
    }
}