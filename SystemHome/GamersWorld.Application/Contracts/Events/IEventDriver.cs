namespace GamersWorld.Application.Contracts.Events;

public interface IEventDriver<TEvent> where TEvent : IEvent
{
    Task Execute(TEvent appEvent);
}