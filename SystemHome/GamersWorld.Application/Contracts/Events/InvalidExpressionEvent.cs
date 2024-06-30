namespace GamersWorld.Application.Contracts.Events;

public class InvalidExpressionEvent : IEvent
{
    public Guid TraceId { get; set; }
    public string? Expression { get; set; }
    public string? Reason { get; set; }
    public DateTime Time { get; set; }
}
