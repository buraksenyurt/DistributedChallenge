using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text;

namespace Resistance.Inconsistency;
public class DataInconsistencyBehavior(RequestDelegate next, DataInconsistencyProbability inconsistencyProbability, ILogger<DataInconsistencyBehavior> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly Random _random = new();
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

        if (isInconsistent && context.Response.ContentType == "application/json")
        {
            _logger.LogWarning("Simulating Data Inconsistency");
            responseBody = AddSomething(responseBody);
        }

        await context.Response.WriteAsync(responseBody);
    }

    private string AddSomething(string originalData)
    {
        try
        {
            using var jsonDocument = JsonDocument.Parse(originalData);
            var jsonObject = jsonDocument.RootElement.Clone();

            using var stream = new MemoryStream();
            using (var writer = new Utf8JsonWriter(stream))
            {
                writer.WriteStartObject();

                foreach (var property in jsonObject.EnumerateObject())
                {
                    property.WriteTo(writer);
                }

                writer.WriteString("Message", "You Shall Not Pass !!!");
                writer.WriteEndObject();
            }

            return Encoding.UTF8.GetString(stream.ToArray());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while adding extra json data.");
            return originalData;
        }
    }
}