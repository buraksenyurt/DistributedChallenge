using System.ComponentModel.DataAnnotations;
using Eval.Api.Requests;
using Eval.Api.Responses;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

/*
    Bu servis Kahin tarafından kullanılan ve sembolik olarak
    talep edilen rapora ait ifadeyi güya kontrol edip geçerli olup
    olmadığına dair bilgi veren bir hizmet sağlar. Söz gelimi rapor ifadesinde
    bilgi güvenliğine aykırı bir durum varsa bunu kontrol eder vb
*/

app.MapPost("/api", (ExpressionCheckRequest request, ILogger<Program> logger) =>
{
    logger.LogInformation("{Source} kaynaklı {Expression} ifadesi kontrol edilecek."
    , request.Source, request.Expression);

    var validationResults = new List<ValidationResult>();
    var validationContext = new ValidationContext(request);

    if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
    {
        logger.LogError("There is some validation errors");
        var errors = validationResults
            .GroupBy(e => e.MemberNames.FirstOrDefault() ?? string.Empty)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage ?? string.Empty).ToArray()
            );

        return Results.ValidationProblem(errors);
    }

    var response = new ExpressionCheckResponse
    {
        IsValid = true
    };
    return Results.Json(response);
})
.WithName("ExpressionCheck")
.WithOpenApi();

app.Run();