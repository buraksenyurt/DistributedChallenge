using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Resistance;

public class LatencyBehavior(RequestDelegate next, LatencyPeriod period, ILogger<LatencyBehavior> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly Random _random = new();
    private readonly LatencyPeriod _period = period;
    private readonly ILogger<LatencyBehavior> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogWarning("Latency simulation applied");
        
        var minDelayMs = (int)_period.MinDelayMs.TotalMilliseconds;
        var maxDelayMs = (int)_period.MaxDelayMs.TotalMilliseconds;

        var delay = _random.Next(minDelayMs, maxDelayMs + 1);
        await Task.Delay(delay);
        await _next(context);
    }
}