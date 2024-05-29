using GamersWorld.SDK.Messages;

namespace GamersWorld.SDK;

public interface IEventExecuter<TEvent>
    where TEvent : IEvent
{
    Task<BusinessResponse> Execute(TEvent appEvent);
}