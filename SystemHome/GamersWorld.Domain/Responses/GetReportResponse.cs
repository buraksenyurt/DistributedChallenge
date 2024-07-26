using GamersWorld.Domain.Enums;

namespace GamersWorld.Domain.Responses;

public class GetReportResponse
{
    public Status StatusCode { get; set; }
    public string? DocumentId { get; set; }
    public byte[]? Document { get; set; }
    public string? Exception { get; set; }
}