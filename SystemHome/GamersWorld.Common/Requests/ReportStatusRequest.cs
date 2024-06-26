namespace GamersWorld.Common.Requests;
public class ReportStatusRequest
{
    public string? TraceId { get; set; }
    public string? EmployeeId { get; set; }
    public string? DocumentId { get; set; }
    public int StatusCode { get; set; }
    public string? StatusMessage { get; set; }
    public string? Detail { get; set; }
}