using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Resistance.Outage;

public class OutageBehavior(RequestDelegate next, ILogger<OutageBehavior> logger, OutagePeriod outagePeriod)
{
    private readonly RequestDelegate _next = next;
    private static bool _serviceOutage = false;
    private static DateTime _outageEndTime;
    private readonly OutagePeriod _outagePeriod = outagePeriod;
    private readonly ILogger<OutageBehavior> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        if (_serviceOutage && DateTime.Now < _outageEndTime)
        {
            _logger.LogWarning("Simulated service outage");
            context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
            await context.Response.WriteAsync("Simulated service outage.");
            return;
        }

        if (!_serviceOutage
            && (DateTime.Now - DateTime.Today).TotalSeconds % _outagePeriod.Frequency.TotalSeconds < _outagePeriod.Duration.TotalSeconds)
        {
            _serviceOutage = true;
            _outageEndTime = DateTime.Now.Add(_outagePeriod.Duration);
        }
        else
        {
            _serviceOutage = false;
        }

        await _next(context);
    }
}