namespace GamersWorld.Domain.Requests;

public class DocumentReadRequest
{
    public Guid TraceId { get; set; }
    public string? DocumentId { get; set; }
    public string? EmployeeId { get; set; }
}