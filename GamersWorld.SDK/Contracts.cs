namespace GamersWorld.SDK;

public interface IEventExecuter<TEvent>
    where TEvent : IEvent
{
    Task<int> Execute(TEvent appEvent);
}