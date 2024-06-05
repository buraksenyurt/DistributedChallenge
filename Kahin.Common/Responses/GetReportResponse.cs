using Kahin.Common.Enums;

namespace Kahin.Common.Responses;

public class GetReportResponse
{
    public StatusCode StatusCode { get; set; }
    public string DocumentId { get; set; }
    public byte[] Document { get; set; }
    public string? Exception { get; set; }
}