namespace GamersWorld.Domain.Requests;

public class GenericDocumentRequest
{
    public Guid TraceId { get; set; }
    public string? DocumentId { get; set; }
    public string? EmployeeId { get; set; }
}