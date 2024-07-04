using GamersWorld.Domain.Enums;
using GamersWorld.Domain.Requests;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SecretsAgent;
using JudgeMiddleware;
using GamersWorld.Application.Contracts.MessageQueue;
using GamersWorld.Application.MessageQueue;
using GamersWorld.Application.Contracts.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddSingleton<ISecretStoreService, SecretStoreService>();
builder.Services.AddSingleton<IEventQueueService, RabbitMqService>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

var app = builder.Build();

app.AddJudgeMiddleware(new Options
{
    DurationThreshold = TimeSpan.FromSeconds(2),
    ExcludedPaths =
    [
        "/swagger"
    ]
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapHealthChecks("/health");

app.MapPost("/", (ReportStatusRequest request, IEventQueueService eventQueueService, ILogger<Program> logger) =>
{
    if (!Guid.TryParse(request.TraceId, out var traceId))
    {
        return Results.BadRequest();
    }

    if (request.StatusCode == (int)StatusCode.ReportReady)
    {
        var reportReadyEvent = new ReportReadyEvent
        {
            TraceId = traceId,
            EmployeeId = request.EmployeeId,
            Time = DateTime.UtcNow,
            CreatedReportId = request.DocumentId,
        };

        eventQueueService.PublishEvent(reportReadyEvent);
        logger.LogInformation(
            "ReporReadyEvent sent. TraceId: {TraceId}, DocumentId: {DocumentId}"
            , traceId, request.DocumentId);
    }
    else if (request.StatusCode == (int)StatusCode.InvalidExpression)
    {
        var invalidExpressionEvent = new InvalidExpressionEvent
        {
            TraceId = traceId,
            Expression = request.Detail,
            Reason = request.StatusMessage,
            Time = DateTime.Now,
        };
        eventQueueService.PublishEvent(invalidExpressionEvent);
        logger.LogError(
            "InvalidExpressionEvent sent. TraceId: {TraceId}, Expression: {Expression}, Reason: {Reason}"
            , traceId, request.Detail, request.StatusMessage);
    }

    return Results.Ok();
})
.WithName("PostReportRequestStatus")
.WithOpenApi();

app.Run();