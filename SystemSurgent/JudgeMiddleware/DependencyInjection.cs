using Microsoft.AspNetCore.Builder;

namespace JudgeMiddleware;

public static class DependencyInjection
{
    public static IApplicationBuilder AddJudgeMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<PerformanceBehavior>();

        return app;
    }
}