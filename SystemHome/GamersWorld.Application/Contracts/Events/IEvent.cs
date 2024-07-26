namespace GamersWorld.Application.Contracts.Events;

public interface IEvent
{
    public BaseEventData EventData { get; set; }
}
