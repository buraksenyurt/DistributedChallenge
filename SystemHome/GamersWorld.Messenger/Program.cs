using System.ComponentModel.DataAnnotations;
using GamersWorld.Common.Requests;
using GamersWorld.Common.Responses;
using GamersWorld.Common.Enums;
using GamersWorld.Events;
using GamersWorld.MQ;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SecretsAgent;
using JudgeMiddleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ISecretStoreService, SecretStoreService>();
builder.Services.AddSingleton<IEventQueueService, RabbitMqService>();
builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

app.AddJudgeMiddleware(new MetricOptions
{
    DurationThreshold = TimeSpan.FromSeconds(2)
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapHealthChecks("/health");

app.MapPost("/", (NewReportRequest request, IEventQueueService eventQueueService, ILogger<Program> logger) =>
{
    var validationResults = new List<ValidationResult>();
    var validationContext = new ValidationContext(request);

    if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
    {
        logger.LogError("Validation errors occurred!");
        var errors = validationResults
            .GroupBy(e => e.MemberNames.FirstOrDefault() ?? string.Empty)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage ?? string.Empty).ToArray()
            );

        var errorResponse = new BusinessResponse
        {
            StatusCode = StatusCode.ValidationErrors,
            Message = "Validation errors occurred.",
            ValidationErrors = errors
        };

        return Results.Json(errorResponse, statusCode: 400);
    }

    var reportRequestedEvent = new ReportRequestedEvent
    {
        TraceId = Guid.NewGuid(),
        Title = request.Title ?? "Last Sales Report",
        Expression = request.Expression ?? "Güncel ülke bazlı satış raporlarının özet dökümü.",
        Time = DateTime.Now,
        EmployeeId = request.EmployeeId
    };

    eventQueueService.PublishEvent(reportRequestedEvent);
    logger.LogInformation(
        "ReportRequestedEvent sent. TraceId: {TraceId}, EmployeeId: {EmployeeId}, Expression: {Expression}"
        , reportRequestedEvent.TraceId, reportRequestedEvent.EmployeeId, reportRequestedEvent.Expression);

    var response = new BusinessResponse
    {
        StatusCode = StatusCode.Success,
        Message = "Successfully sent"
    };
    return Results.Json(response);
})
.WithName("NewReportRequest")
.WithOpenApi();

app.Run();