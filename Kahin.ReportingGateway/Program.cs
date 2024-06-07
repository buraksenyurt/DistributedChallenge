using System.ComponentModel.DataAnnotations;
using Kahin.Common.Entities;
using Kahin.Common.Enums;
using Kahin.Common.Requests;
using Kahin.Common.Responses;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/health");

app.MapPost("/getReport", async (GetReportRequest request, ILogger<Program> logger) =>
{
    var response = new GetReportResponse();
    try
    {
        var documentId = ReferenceDocumentId.Parse(request.DocumentId);
        logger.LogWarning("{DocumentId} nolu rapor verilecek", request.DocumentId);
        // Önceden hazırlanmış raporlar için Redis tabanlı bir caching konulabilir

        var reportContent = await File.ReadAllBytesAsync("SampleReport.dat");

        response = new GetReportResponse
        {
            DocumentId = documentId.ToString(),
            Document = reportContent,
            StatusCode = StatusCode.ReportReady,
        };
    }
    catch (Exception excp)
    {
        logger.LogError("{ExceptionMessage}", excp.Message);
        response = new GetReportResponse
        {
            Exception = excp.Message,
            StatusCode = StatusCode.Fail
        };
    }

    return Results.Json(response);
})
.WithName("GetReport")
.WithOpenApi();

app.MapPost("/", (CreateReportRequest request, ILogger<Program> logger) =>
{
    logger.LogInformation("{TraceId}, {Title}, {Expression}", request.TraceId, request.Title, request.Expression);
    var validationResults = new List<ValidationResult>();
    var validationContext = new ValidationContext(request);

    if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
    {
        var errors = validationResults
            .GroupBy(e => e.MemberNames.FirstOrDefault() ?? string.Empty)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage ?? string.Empty).ToArray()
            );

        return Results.ValidationProblem(errors);
    }

    if (!Guid.TryParse(request.TraceId, out var traceId))
    {
        logger.LogWarning("TraceId must be a valid GUID.");
        return Results.BadRequest(new { error = "TraceId must be a valid GUID." });
    }

    Random rnd = new();
    // Gelen talepteki bilgilere göre rapor talebini benzersiz bir veri modeli ile damgalamak istiyoruz
    var refDocId = new ReferenceDocumentId
    {
        Head = rnd.Next(1000, 1100),
        Source = rnd.Next(1, 10),
        Stamp = Guid.NewGuid(),
    };

    logger.LogInformation("Created Referenced Document Id: {RefDocumentId}", refDocId.ToString());

    // Bu sistem kendi için rapor hazırlama işini başlatıyor şeklinde düşünelim.
    // Request üzerinden gelen Expression içeriğinin de Gen AI tarzı bir API ile bu sistemde 
    // anlamlı ve işletilebilir bir ifadeye dönüştürüldüğünü düşünelim.
    // Şu an için test amaçlı sabit bir response döndürmesi yeterli

    var response = new CreateReportResponse
    {
        Status = StatusCode.Success,
        DocumentId = refDocId.ToString(),
        Explanation = "Rapor talebi başarılı bir şekilde alındı"
    };
    return Results.Json(response);
})
.WithName("CreateReportRequest")
.WithOpenApi();

app.Run();