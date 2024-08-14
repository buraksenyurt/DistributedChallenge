using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resistance.Configuration;
using System.Net;

namespace Resistance.ResourceRace;
public class ResourceRaceBehavior
{
    private readonly RequestDelegate _next;
    private static SemaphoreSlim _semaphore = new(2);
    private readonly ILogger<ResourceRaceBehavior> _logger;
    private readonly IOptionsMonitor<ResistanceFlags> _optionsMonitor;

    public ResourceRaceBehavior(RequestDelegate next, IOptionsMonitor<ResistanceFlags> optionsMonitor, ILogger<ResourceRaceBehavior> logger, ushort upperLimit)
    {
        _next = next;
        _semaphore = new SemaphoreSlim(upperLimit);
        _logger = logger;
        _optionsMonitor = optionsMonitor;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!_optionsMonitor.CurrentValue.ResourceRaceIsActive)
        {
            await _next(context);
            return;
        }

        if (!await _semaphore.WaitAsync(0))
        {
            _logger.LogWarning("Simulated TooManyRequest(HTTP 429)");
            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            await context.Response.WriteAsync("Simulated resource contention.");
            return;
        }

        try
        {
            await _next(context);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
