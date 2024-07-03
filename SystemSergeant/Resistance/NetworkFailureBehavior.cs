using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Resistance;

public class NetworkFailureBehavior(RequestDelegate next, NetworkFailureProbability failureProbability, ILogger<NetworkFailureBehavior> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly Random _random = new Random();
    private readonly int _failureProbability = (int)failureProbability;
    private readonly ILogger<NetworkFailureBehavior> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        if (_random.Next(1, 101) <= _failureProbability)
        {
            _logger.LogWarning("Simulated newtwork failure with HTTP 500 code.");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync("Simulated network failure.");
        }
        else
        {
            await _next(context);
        }
    }
}