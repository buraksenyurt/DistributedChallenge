using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Resistance;

public class NetworkFailureBehavior
{
    private readonly RequestDelegate _next;
    private readonly Random _random;
    private readonly ILogger<NetworkFailureBehavior> _logger;

    public NetworkFailureBehavior(RequestDelegate next, ILogger<NetworkFailureBehavior> logger)
    {
        _next = next;
        _random = new Random();
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Simulate network failure with a 10% probability
        if (_random.Next(1, 11) <= 1)
        {
            _logger.LogWarning("Simulated newtwork failure...");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync("Simulated network failure.");
        }
        else
        {
            await _next(context);
        }
    }
}