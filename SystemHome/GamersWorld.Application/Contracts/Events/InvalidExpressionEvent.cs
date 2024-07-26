namespace GamersWorld.Application.Contracts.Events;

public class InvalidExpressionEvent : IEvent
{
    public BaseEventData EventData { get; set; } = new BaseEventData();
    public string? Expression { get; set; }
    public string? Reason { get; set; }
    public string? EmployeeId { get; set; }
    public string? Title { get; set; }

}
