namespace GamersWorld.Application.Contracts.Events;
public class ReportReadyEvent : IEvent
{
    public BaseEventData EventData { get; set; } = new BaseEventData();
    public string? EmployeeId { get; set; }
    public string? Title { get; set; }
    public string? Expression { get; set; }
    public TimeSpan ExpireTime { get; set; }
    public string? CreatedReportId { get; set; }
}