using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Resistance;
public class ResourceRaceBehavior
{
    private readonly RequestDelegate _next;
    private static SemaphoreSlim _semaphore = new(2);
    private readonly ILogger<ResourceRaceBehavior> _logger;

    public ResourceRaceBehavior(RequestDelegate next, ILogger<ResourceRaceBehavior> logger, ushort upperLimit)
    {
        _next = next;
        _semaphore = new SemaphoreSlim(upperLimit);
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
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
