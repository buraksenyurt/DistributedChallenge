using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace JudgeMiddleware;

public class PerformanceBehavior(RequestDelegate next, ILogger<PerformanceBehavior> logger, Options options)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<PerformanceBehavior> _logger = logger;
    private readonly Options _options = options;

    public async Task Invoke(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();
        var responseTime = stopwatch.ElapsedMilliseconds;

        if (responseTime > _options.DurationThreshold.TotalMilliseconds)
        {
            _logger.LogWarning("Request {Method} {Path} took {ResponseTime}(ms) which is above the threshold of {Threshold}(ms)",
                context.Request.Method, context.Request.Path, responseTime, _options.DurationThreshold.TotalMilliseconds);
        }
        else
        {
            _logger.LogInformation("Request {Method} {Path} took {ResponseTime}(ms)",
                context.Request.Method, context.Request.Path, responseTime);
        }
    }
}
