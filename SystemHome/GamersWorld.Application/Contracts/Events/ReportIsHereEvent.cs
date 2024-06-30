namespace GamersWorld.Application.Contracts.Events;

public class ReportIsHereEvent : IEvent
{
    public Guid TraceId { get; set; }
    public string? EmployeeId { get; set; }
    public DateTime Time { get; set; }
    public string? CreatedReportId { get; set; }
}
