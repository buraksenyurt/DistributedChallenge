using System.ComponentModel.DataAnnotations;
using GamersWorld.Domain.Requests;
using GamersWorld.Domain.Responses;
using GamersWorld.Domain.Enums;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SecretsAgent;
using JudgeMiddleware;
using GamersWorld.Repository;
using Microsoft.AspNetCore.Mvc;
using GamersWorld.Application.Contracts.Document;
using GamersWorld.Application.Contracts.MessageQueue;
using GamersWorld.Application.MessageQueue;
using GamersWorld.Application.Contracts.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IDocumentRepository, DocumentRepository>();
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

app.MapGet("/", async ([FromQuery] string employeeId, IDocumentRepository repository, ILogger<Program> logger) =>
{
    var documents = await repository.GetAllDocumentsByEmployeeAsync(new DocumentReadRequest
    {
        EmployeeId = employeeId,
    });

    return Results.Json(documents);
})
.WithName("GetReportsByEmployee")
.WithOpenApi();


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
        EmployeeId = request.EmployeeId,
        Title = request.Title ?? "Last Sales Report",
        Expression = request.Expression ?? "Güncel ülke bazlı satış raporlarının özet dökümü.",
        Time = DateTime.Now,
    };

    eventQueueService.PublishEvent(reportRequestedEvent);
    logger.LogInformation(
        "ReportRequestedEvent sent. TraceId: {TraceId}, Expression: {Expression}"
        , reportRequestedEvent.TraceId, reportRequestedEvent.Expression);

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