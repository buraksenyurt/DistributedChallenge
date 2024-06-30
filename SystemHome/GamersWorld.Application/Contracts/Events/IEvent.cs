namespace GamersWorld.Application.Contracts.Events;

public interface IEvent
{
    Guid TraceId { get; set; }
    DateTime Time { get; set; }
}
