namespace GamersWorld.Application.Contracts.Events;

public class DeleteReportRequestEvent : IEvent
{
    public BaseEventData EventData { get; set; } = new BaseEventData();
    public string? DocumentId { get; set; }
    public string? Title { get; set; }
    public string? ClientId { get; set; }
}
