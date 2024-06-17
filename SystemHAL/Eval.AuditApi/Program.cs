using System.ComponentModel.DataAnnotations;
using System.Data;
using Eval.AuditLib.Contracts;
using Eval.AuditLib.Model;
using Eval.Lib;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IExpressionValidator, ExpressionValidator>();
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

/*
    Bu servis Kahin tarafından kullanılan ve sembolik olarak
    talep edilen rapora ait ifadeyi güya kontrol edip geçerli olup
    olmadığına dair bilgi veren bir hizmet sağlar. Söz gelimi rapor ifadesinde
    bilgi güvenliğine aykırı bir durum varsa bunu kontrol eder vb
*/

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