using System.ComponentModel.DataAnnotations;
using GamersWorld.Common.Messages.Requests;
using GamersWorld.Common.Messages.Responses;
using GamersWorld.Common.Enums;
using GamersWorld.Common.Settings;
using GamersWorld.AppEvents;
using GamersWorld.MQ;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddSingleton<IEventQueueService, RabbitMqService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

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
        Title = request.Title,
        Expression = request.Expression,
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