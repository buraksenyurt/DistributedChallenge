namespace GamersWorld.Domain.Requests;
public class UpdateReportStatusRequest
{
    public string? TraceId { get; set; }
    public string? EmployeeId { get; set; }
    public string? ReportTitle { get; set; }
    public string? Expression { get; set; }
    public string? DocumentId { get; set; }
    public int StatusCode { get; set; }
    public string? StatusMessage { get; set; }
    public string? Detail { get; set; }
    public TimeSpan ExpireTime { get; set; }
}