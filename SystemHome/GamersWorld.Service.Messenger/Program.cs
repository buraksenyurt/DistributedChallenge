using GamersWorld.Application;
using GamersWorld.Application.Contracts.Data;
using GamersWorld.Application.Contracts.Events;
using GamersWorld.Application.Contracts.MessageQueue;
using GamersWorld.Domain.Dtos;
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

var documentsGroup = app.MapGroup("/api/documents");

documentsGroup.MapGet("/employee/{employeeId}", async (string employeeId, IReportDataRepository repository, ILogger<Program> logger) =>
{
    logger.LogInformation("Request reports data for {EmployeeId}", employeeId);
    var documents = await repository.ReadAllReportsAsync(employeeId);

    return Results.Json(documents);
})
.WithName("GetReportsByEmployee")
.WithOpenApi();

documentsGroup.MapGet("/{documentId}", async (string documentId, IReportDocumentDataRepository repository, ILogger<Program> logger) =>
{
    logger.LogInformation("Request report content for {DocumentId}", documentId);
    var document = await repository.ReadDocumentAsync(documentId);

    if (document == null)
    {
        var errorResponse = new BusinessResponse
        {
            Status = Status.DocumentNotFound,
            Message = "Document not found",
            ValidationErrors = null
        };

        return Results.Json(errorResponse, statusCode: 404);
    }

    return Results.Json(new DocumentContentDto
    {
        Base64Content = Convert.ToBase64String(document.Content ?? []),
        ContentSize = document.Content.Length
    });
})
.WithName("GetReportContentById")
.WithOpenApi();


documentsGroup.MapPost("/", (NewReportRequest request, IEventQueueService eventQueueService, ILogger<Program> logger) =>
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
            Status = Status.ValidationErrors,
            Message = "Validation errors occurred.",
            ValidationErrors = errors
        };

        return Results.Json(errorResponse, statusCode: 400);
    }

    var reportRequestedEvent = new ReportRequestedEvent
    {
        EventData = new BaseEventData
        {
            TraceId = Guid.NewGuid(),
            Time = DateTime.Now
        },
        EmployeeId = request.EmployeeId,
        Title = request.Title ?? "Last Sales Report",
        Expression = request.Expression ?? "Güncel ülke bazlı satış raporlarının özet dökümü.",
        Lifetime = request.Lifetime,
    };

    eventQueueService.PublishEvent(reportRequestedEvent);
    logger.LogInformation(
        "ReportRequestedEvent sent. TraceId: {TraceId}, Expression: {Expression}"
        , reportRequestedEvent.EventData.TraceId, reportRequestedEvent.Expression);

    var response = new BusinessResponse
    {
        Status = Status.Success,
        Message = "Successfully sent"
    };
    return Results.Json(response);
})
.WithName("NewReportRequest")
.WithOpenApi();

documentsGroup.MapDelete("/{documentId}", (string documentId, [FromBody] DeleteReportRequest request, IEventQueueService eventQueueService, ILogger<Program> logger) =>
{
    logger.LogInformation("Delete report request for {DocumentId}", request.DocumentId);
    var deleteReportEvent = new DeleteReportRequestEvent
    {
        EventData = new BaseEventData
        {
            TraceId = Guid.NewGuid(),
            Time = DateTime.Now
        },
        DocumentId = request.DocumentId,
        ClientId = request.EmployeeId,
        Title = request.Title
    };
    eventQueueService.PublishEvent(deleteReportEvent);

    return Results.Accepted();
})
.WithName("DeleteReportById")
.WithOpenApi();

documentsGroup.MapPost("/archive", (ArchiveReportRequest request, IEventQueueService eventQueueService, ILogger<Program> logger) =>
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
            Status = Status.ValidationErrors,
            Message = "Validation errors occurred.",
            ValidationErrors = errors
        };

        return Results.Json(errorResponse, statusCode: 400);
    }

    var archiveReportEvent = new ArchiveReportRequestEvent
    {
        EventData = new BaseEventData
        {
            TraceId = Guid.NewGuid(),
            Time = DateTime.Now
        },
        DocumentId = request.DocumentId,
        ClientId = request.EmployeeId,
        Title = request.Title
    };

    eventQueueService.PublishEvent(archiveReportEvent);
    logger.LogInformation(
        "ArchiveReportEvent sent. DocumentId: {DocumentId}, Title: {Title}"
        , archiveReportEvent.DocumentId, archiveReportEvent.Title);

    var response = new BusinessResponse
    {
        Status = Status.Success,
        Message = "Successfully sent"
    };
    return Results.Json(response);
})
.WithName("ArchiveReportRequest")
.WithOpenApi();

app.Run();