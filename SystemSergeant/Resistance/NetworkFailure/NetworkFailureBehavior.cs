using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resistance.Configuration;

namespace Resistance.NetworkFailure;

public class NetworkFailureBehavior(
    RequestDelegate next
    , NetworkFailureProbability failureProbability
    , IOptionsMonitor<ResistanceFlags> optionsMonitor
    , ILogger<NetworkFailureBehavior> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly Random _random = new();
    private readonly int _failureProbability = (int)failureProbability;
    private readonly ILogger<NetworkFailureBehavior> _logger = logger;
    private readonly IOptionsMonitor<ResistanceFlags> _optionsMonitor = optionsMonitor;
    public async Task InvokeAsync(HttpContext context)
    {
        if (!_optionsMonitor.CurrentValue.NetworkFailureIsActive)
        {
            await _next(context);
            return;
        }

        if (_random.Next(1, 101) <= _failureProbability)
        {
            _logger.LogError("Simulated newtwork failure with HTTP 500 code.");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync("Simulated network failure.");
        }
        else
        {
            await _next(context);
        }
    }
}