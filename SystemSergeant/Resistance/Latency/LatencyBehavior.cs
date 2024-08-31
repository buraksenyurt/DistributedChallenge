using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resistance.Configuration;

namespace Resistance.Latency;

public class LatencyBehavior(
    RequestDelegate next,
    LatencyPeriod period,
    ILogger<LatencyBehavior> logger,
    IOptionsMonitor<ResistanceFlags> optionsMonitor)
{
    private readonly RequestDelegate _next = next;
    private readonly Random _random = new();
    private readonly LatencyPeriod _period = period;
    private readonly ILogger<LatencyBehavior> _logger = logger;
    private readonly IOptionsMonitor<ResistanceFlags> _optionsMonitor = optionsMonitor;

    public async Task InvokeAsync(HttpContext context)
    {
        if (!_optionsMonitor.CurrentValue.LatencyIsActive)
        {
            await _next(context);
            return;
        }

        _logger.LogError("Latency simulation applied");

        var minDelayMs = (int)_period.MinDelayMs.TotalMilliseconds;
        var maxDelayMs = (int)_period.MaxDelayMs.TotalMilliseconds;

        var delay = _random.Next(minDelayMs, maxDelayMs + 1);
        await Task.Delay(delay);
        await _next(context);
    }
}