namespace GamersWorld.SDK;

public interface IEventDriver<TEvent> where TEvent : IEvent
{
    Task Execute(TEvent appEvent);
}