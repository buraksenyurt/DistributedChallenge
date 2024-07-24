namespace GamersWorld.Domain.Requests;

public class ReportSaveRequest
{
    public Guid TraceId { get; set; }
    public string? Title { get; set; }
    public string? Expression { get; set; }
    public string? EmployeeId { get; set; }
    public string? DocumentId { get; set; }
    public byte[]? Content { get; set; }
    public DateTime InsertTime { get; set; }
    public DateTime ExpireTime { get; set; }
}