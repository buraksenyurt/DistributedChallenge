using GamersWorld.Application;
using GamersWorld.Application.Contracts.Document;
using GamersWorld.Application.Contracts.Events;
using GamersWorld.Application.Contracts.MessageQueue;
using GamersWorld.Domain.Enums;
using GamersWorld.Domain.Requests;
using GamersWorld.Domain.Responses;
using GamersWorld.Repository;
using JudgeMiddleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Consul;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication();
builder.Services.AddData();
builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddServiceDiscovery(o => o.UseConsul());
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

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

app.MapGet("/", async ([FromQuery] string employeeId, IDocumentRepository repository, ILogger<Program> logger) =>
{
    logger.LogInformation("Request reports data for {EmployeeId}", employeeId);
    var documents = await repository.GetAllDocumentsByEmployeeAsync(new DocumentReadRequest
    {
        EmployeeId = employeeId,
    });

    return Results.Json(documents);
})
.WithName("GetReportsByEmployee")
.WithOpenApi();

app.MapGet("/document", async ([FromQuery] string documentId, IDocumentRepository repository, ILogger<Program> logger) =>
{
    logger.LogInformation("Request report content for {DocumentId}", documentId);
    var document = await repository.ReadDocumentContentByIdAsync(
        new DocumentReadRequest
        {
            DocumentId = documentId
        });
    return Results.Json(document);
})
.WithName("GetReportContentById")
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

app.MapDelete("/document", async ([FromQuery] string documentId, IDocumentRepository repository, ILogger<Program> logger) =>
{
    logger.LogInformation("Delete report request for {DocumentId}", documentId);
    var affectedRowCount = await repository.DeleteDocumentByIdAsync(
        new DocumentReadRequest
        {
            DocumentId = documentId
        });
    if (affectedRowCount == 0)
        return Results.NotFound();

    return Results.Ok();
})
.WithName("DeleteReportById")
.WithOpenApi();

app.Run();