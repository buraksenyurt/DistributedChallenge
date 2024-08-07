using GamersWorld.Domain.Enums;
using GamersWorld.Domain.Requests;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SecretsAgent;
using JudgeMiddleware;
using GamersWorld.Application.Contracts.MessageQueue;
using GamersWorld.Application.MessageQueue;
using GamersWorld.Application.Contracts.Events;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Consul;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddSingleton<ISecretStoreService, SecretStoreService>();
builder.Services.AddSingleton<IEventQueueService, RabbitMqService>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddServiceDiscovery(o => o.UseConsul());

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

app.MapPost("/", (UpdateReportStatusRequest request, IEventQueueService eventQueueService, ILogger<Program> logger) =>
{
    if (!Guid.TryParse(request.TraceId, out var traceId))
    {
        return Results.BadRequest();
    }

    if (request.StatusCode == (int)Status.ReportReady)
    {
        var reportReadyEvent = new ReportReadyEvent
        {
            EventData = new BaseEventData
            {
                TraceId = traceId,
                Time = DateTime.Now
            },
            EmployeeId = request.EmployeeId,
            Title = request.ReportTitle,
            Expression = request.Expression,
            CreatedReportId = request.DocumentId,
            ExpireTime = request.ExpireTime
        };

        eventQueueService.PublishEvent(reportReadyEvent);
        logger.LogInformation(
            "ReporReadyEvent sent. TraceId: {TraceId}, DocumentId: {DocumentId}"
            , traceId, request.DocumentId);
    }
    else if (request.StatusCode == (int)Status.InvalidExpression)
    {
        var invalidExpressionEvent = new InvalidExpressionEvent
        {
            EventData = new BaseEventData
            {
                TraceId = traceId,
                Time = DateTime.Now
            },
            Expression = request.Detail,
            Reason = request.StatusMessage,
            EmployeeId = request.EmployeeId,
        };
        eventQueueService.PublishEvent(invalidExpressionEvent);
        logger.LogError(
            "InvalidExpressionEvent sent. TraceId: {TraceId}, Expression: {Expression}, Reason: {Reason}"
            , traceId, request.Detail, request.StatusMessage);
    }

    return Results.Ok();
})
.WithName("NewReportRequestStatus")
.WithOpenApi();

app.Run();