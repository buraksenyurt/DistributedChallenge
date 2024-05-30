using GamersWorld.AppEvents;
using GamersWorld.GateWayProxy;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var rabbitMqSettings = builder.Configuration.GetSection("RabbitMqSettings").Get<RabbitMqSettings>();
builder.Services.AddSingleton(sp => new RabbitMqService(rabbitMqSettings));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

/*
    System ABC' ye (Kahin domain'i diyelim) bir rapor talebi geldikten sonra o da
    bu sistemi (System 123) GateWayProxy servisi araclığıyla besleyebilir.

    Örneğin aşağıdaki Post metodunu rapor hazır olduğunda veya raporla ilgili bir 
    doğrulama ihlali söz konusu olduğunda kullanır.

    Bu serviste System 123 içerisindeki event queue'yu kullanarak gerekli olay bilgilendirmelerini yapar

*/

app.MapPost("/", (ReportStatusRequest request, RabbitMqService rabbitMQService) =>
{
    if (request.StatusCode == (int)StatusCode.ReportReady)
    {
        var reportReadyEvent = new ReportReadyEvent
        {
            TraceId = Guid.Parse(request.TraceId),
            Time = DateTime.UtcNow,
            CreatedReportId = request.DocumentId,
        };

        rabbitMQService.PublishEvent(reportReadyEvent);
    }
    else if (request.StatusCode == (int)StatusCode.InvalidExpression)
    {
        var invalidExpressionEvent = new InvalidExpressionEvent
        {
            TraceId = Guid.Parse(request.TraceId),
            Expression = request.Detail,
            Reason = request.StatusMessage,
            Time = DateTime.Now,
        };
        rabbitMQService.PublishEvent(invalidExpressionEvent);
    }

    return Results.Ok();
})
.WithName("PostReportRequestStatus")
.WithOpenApi();

app.Run();

class ReportStatusRequest
{
    public string TraceId { get; set; }
    public string DocumentId { get; set; }
    public int StatusCode { get; set; }
    public string StatusMessage { get; set; }
    public string Detail { get; set; }
}

enum StatusCode
{
    ReportReady = 200,
    InvalidExpression = 400
}