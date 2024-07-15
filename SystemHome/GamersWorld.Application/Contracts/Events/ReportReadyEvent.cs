namespace GamersWorld.Application.Contracts.Events;
public class ReportReadyEvent : IEvent
{
    public Guid TraceId { get; set; }
    public string? EmployeeId { get; set; }
    public string? Title { get; set; }
    public DateTime Time { get; set; }
    public TimeSpan ExpireTime { get; set; }
    public string? CreatedReportId { get; set; }
}