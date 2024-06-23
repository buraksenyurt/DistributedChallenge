namespace GamersWorld.Common.Requests;

public class DocumentReadRequest
{
    public Guid TraceId { get; set; }
    public string? DocumentId { get; set; }
}