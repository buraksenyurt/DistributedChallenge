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

var rabbitMqSettings = builder.Configuration.GetSection("RabbitMqSettings").Get<RabbitMqSettings>();
builder.Services.AddSingleton(sp => new RabbitMqService(rabbitMqSettings));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/", (NewReportRequest request, RabbitMqService rabbitMQService, ILogger<Program> logger) =>
{
    var validationResults = new List<ValidationResult>();
    var validationContext = new ValidationContext(request);

    if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
    {
        logger.LogError("Doğrulama hataları var!");
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

    rabbitMQService.PublishEvent(reportRequestedEvent);
    logger.LogInformation(
        "ReportRequestedEvent gönderildi. TraceId: {TraceId}, Expression: {Expression}"
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