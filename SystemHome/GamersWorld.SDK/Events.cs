namespace GamersWorld.SDK;

public interface IEvent
{
    Guid TraceId { get; set; }
    DateTime Time { get; set; }
}
