using System.ComponentModel.DataAnnotations;
using System.Data;
using Eval.AuditLib.Contracts;
using Eval.AuditLib.Model;
using Eval.Lib;
using JudgeMiddleware;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Resistance;
using Resistance.Inconsistency;
using Resistance.Latency;
using Resistance.NetworkFailure;
using Resistance.Outage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IExpressionValidator, ExpressionValidator>();
builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());
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

// Latency 500 millisecdons - 2500 milliseconds
//app.UseResistance(new ResistanceOptions
//{
//    LatencyIsActive = true,
//    LatencyPeriod = new LatencyPeriod
//    {
//        MinDelayMs = TimeSpan.FromMilliseconds(500),
//        MaxDelayMs = TimeSpan.FromMilliseconds(2500)
//    }
//});

//// Network Failure (HTTP 500 Internal Service Error with %25 probility)
//app.UseResistance(new ResistanceOptions
//{
//    NetworkFailureIsActive = true,
//    NetworkFailureProbability = NetworkFailureProbability.Percent25
//});

//// Produce HTTP 429 Too Many Request scenario with 3 concurrent request
//app.UseResistance(new ResistanceOptions
//{
//    ResourceRaceIsActive = true,
//    ResourceRaceUpperLimit = 3
//});

//// Manipulating response data with %50 probability (MUST BE TESTED AGAIN)
//app.UseResistance(new ResistanceOptions
//{
//    DataInconsistencyIsActive = true,
//    DataInconsistencyProbability = DataInconsistencyProbability.Percent20
//});

//// Produce HTTP 503 Service Unavailable 10 seconds per minute
//app.UseResistance(new ResistanceOptions
//{
//    OutageIsActive = true,
//    OutagePeriod = new OutagePeriod
//    {
//        Duration = TimeSpan.FromSeconds(10),
//        Frequency = TimeSpan.FromMinutes(1)
//    }
//});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapHealthChecks("/health");

app.MapPost("/api", (ExpressionCheckRequest request, ILogger<Program> logger, IExpressionValidator validator) =>
{
    logger.LogInformation("{Source}, '{Expression}' is under audit check."
    , request.Source, request.Expression);

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

        return Results.ValidationProblem(errors);
    }

    var response = validator.IsValid(request);
    return Results.Json(response);
})
.WithName("ExpressionCheck")
.WithOpenApi();

app.Run();