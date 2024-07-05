using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Resistance.Inconsistency;
public class DataInconsistencyBehavior(RequestDelegate next, DataInconsistencyProbability inconsistencyProbability, ILogger<DataInconsistencyBehavior> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly Random _random = new Random();
    private readonly DataInconsistencyProbability _inconsistencyProbability = inconsistencyProbability;
    private readonly ILogger<DataInconsistencyBehavior> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        bool isInconsistent = _random.Next(1, 101) <= (int)_inconsistencyProbability;
        context.Response.Headers["Data-Inconsistency"] = isInconsistent ? "true" : "false";

        var originalBodyStream = context.Response.Body;
        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        await _next(context);

        context.Response.Body = originalBodyStream;
        responseBodyStream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();
        responseBodyStream.Seek(0, SeekOrigin.Begin);

        if (isInconsistent)
        {
            _logger.LogWarning("Simulating Data Inconsistency");
            responseBody += "\n<!-- Inconsistent Data -->";
        }

        await context.Response.WriteAsync(responseBody);
    }
}