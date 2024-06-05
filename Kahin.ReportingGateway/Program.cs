using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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

app.MapGet("/", (GetReportRequest request, ILogger<Program> logger) =>
{
    var response = new GetReportResponse();
    try
    {
        var documentId = ReferenceDocumentId.Parse(request.DocumentId);
        logger.LogWarning("{DocumentId} nolu rapor verilecek", request.DocumentId);
        // Sonrasında önceden hazırlanmış raporlar için Redis tabanlı bir caching konulabilir
        // Sistemden bu numaraya ait rapor dokümanının çekildiğini düşünelim
        response = new GetReportResponse
        {
            DocumentId = documentId.ToString(),
            Document = new byte[1024],
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

class CreateReportRequest
{
    [Required]
    public string? TraceId { get; set; }

    [Required]
    [StringLength(30, MinimumLength = 20, ErrorMessage = "Title length must be between 20 and 30 characters.")]
    public string? Title { get; set; }

    [Required(ErrorMessage = "Expression must be filled.")]
    [StringLength(100, MinimumLength = 30, ErrorMessage = "Expression length must be between 30 and 100 characters.")]
    public string? Expression { get; set; }
}


enum StatusCode
{
    Success = 1,
    ReportReady = 200,
    InvalidExpression = 400,
    Fail = 500
}

struct ReferenceDocumentId
{
    public int Head { get; set; }
    public int Source { get; set; }
    public Guid Stamp { get; set; }
    public override readonly string ToString()
    {
        return $"{Head}-{Source}-{Stamp}";
    }
    public static ReferenceDocumentId Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentNullException(nameof(input), "Input string cannot be null or empty.");
        }

        var parts = input.Split('-');
        if (parts.Length != 3)
        {
            throw new FormatException("Input string must be in the format 'Head-Source-Stamp'.");
        }

        if (!int.TryParse(parts[0], out int head))
        {
            throw new FormatException("Head part must be a valid integer.");
        }

        if (!int.TryParse(parts[1], out int source))
        {
            throw new FormatException("Source part must be a valid integer.");
        }

        if (!Guid.TryParse(parts[2], out Guid stamp))
        {
            throw new FormatException("Stamp part must be a valid GUID.");
        }

        return new ReferenceDocumentId
        {
            Head = head,
            Source = source,
            Stamp = stamp
        };
    }
}

class CreateReportResponse
{
    public StatusCode Status { get; set; }

    [JsonIgnore]
    public ReferenceDocumentId ReferenceDocumentId { get; set; }

    public string? DocumentId { get; set; }
    public string? Explanation { get; set; }
}

class GetReportRequest
{
    [Required]
    public string DocumentId { get; set; }
}

class GetReportResponse
{
    public StatusCode StatusCode { get; set; }
    public string DocumentId { get; set; }
    public byte[] Document { get; set; }
    public string Exception { get; set; }
}