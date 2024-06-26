namespace GamersWorld.Common.Requests;

public class DocumentSaveRequest
{
    public Guid TraceId { get; set; }
    public string? DocumentId { get; set; }
    public byte[]? Content { get; set; }
}