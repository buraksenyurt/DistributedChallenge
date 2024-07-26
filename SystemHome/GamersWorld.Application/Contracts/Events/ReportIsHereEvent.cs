namespace GamersWorld.Application.Contracts.Events;

public class ReportIsHereEvent : IEvent
{
    public BaseEventData EventData { get; set; } = new BaseEventData();
    public string? Title { get; set; }
    public string? EmployeeId { get; set; }
    public string? CreatedReportId { get; set; }
    public string? Expression { get; internal set; }
}
