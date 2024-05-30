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

app.MapPost("/", (ReportStatusRequest request) =>
{
    if (request.StatusCode == (int)StatusCode.ReportReady)
    {

    }
    else if (request.StatusCode == (int)StatusCode.InvalidExpression)
    {

    }

    return Results.Ok();
})
.WithName("PostReportRequestStatus")
.WithOpenApi();

app.Run();

class ReportStatusRequest
{
    public ReferenceDocumentId ReferenceDocumentId { get; set; }
    public int StatusCode { get; set; }
    public string StatusMessage { get; set; }
}

enum StatusCode
{
    ReportReady = 200,
    InvalidExpression = 400
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