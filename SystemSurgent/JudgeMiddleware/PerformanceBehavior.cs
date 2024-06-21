using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace JudgeMiddleware;

public class PerformanceBehavior(RequestDelegate next, ILogger<PerformanceBehavior> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<PerformanceBehavior> _logger = logger;

    public async Task Invoke(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();
        var responseTime = stopwatch.ElapsedMilliseconds;
        _logger.LogInformation("Response Time: {ResponseTime} ms", responseTime);
    }
}
