using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/", (CreateReportRequest request) =>
{
    // Gelen talepteki bilgilere göre rapor talebini benzersiz bir veri modeli ile damgalamak istiyoruz
    var refDocId = new ReferenceDocumentId
    {
        Head = 1001,
        Source = 23,
        Stamp = Guid.Parse(request.TraceId)
    };

    // Bu sistem kendi için rapor hazırlama işini başlatıyor şeklinde düşünelim.
    // Şu an için test amaçlı sabit bir response döndürmesi yeterli

    var response = new CreateReportResponse
    {
        Status = StatusCode.Success,
        DocumentId = refDocId.ToString()
    };
    return Results.Json(response);
})
.WithName("CreateReportRequest")
.WithOpenApi();

app.Run();

class CreateReportRequest
{
    public string TraceId { get; set; }
    public string Title { get; set; }
    public string Expression { get; set; }
}

enum StatusCode
{
    Success = 200,
    Error = 400
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
}

class CreateReportResponse
{
    public StatusCode Status { get; set; }

    [JsonIgnore]
    public ReferenceDocumentId ReferenceDocumentId { get; set; }

    public string DocumentId { get; set; }
}