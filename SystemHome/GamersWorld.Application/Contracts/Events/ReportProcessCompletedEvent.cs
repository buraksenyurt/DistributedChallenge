namespace GamersWorld.Application.Contracts.Events;

public class ReportProcessCompletedEvent : IEvent
{
    public BaseEventData EventData { get; set; } = new BaseEventData();
    public Guid CreatedReportId { get; set; }
}
