namespace JudgeMiddleware;

using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class InputOutputBehavior(RequestDelegate next, ILogger<InputOutputBehavior> logger, Options options)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<InputOutputBehavior> _logger = logger;
    private readonly Options _options = options;

    public async Task Invoke(HttpContext context)
    {
        if (_options.ExcludedPaths.Any(path => context.Request.Path.StartsWithSegments(path)))
        {
            await _next(context);
            return;
        }

        var request = await FormatRequest(context.Request);
        _logger.LogInformation("Request: {Request}", request);
        var body = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;
        await _next(context);

        var response = await FormatResponse(context.Response);
        _logger.LogInformation("Response: {Response}", response);

        await responseBody.CopyToAsync(body);
    }

    private static async Task<string> FormatRequest(HttpRequest request)
    {
        request.EnableBuffering();
        var body = request.Body;
        var buffer = new byte[Convert.ToInt32(request.ContentLength)];
        await body.ReadAsync(buffer);
        var bodyAsText = Encoding.UTF8.GetString(buffer);
        request.Body.Seek(0, SeekOrigin.Begin);
        return $"{request.Scheme} {request.Host}{request.Path} {request.QueryString} {bodyAsText}";
    }

    private static async Task<string> FormatResponse(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        var text = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);
        return $"StatusCode: {response.StatusCode}, Body: {text}";
    }
}
