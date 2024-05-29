using GamersWorld.SDK.Messages;

namespace GamersWorld.SDK;

public interface IEventDriver<TEvent>
    where TEvent : IEvent
{
    Task<BusinessResponse> Execute(TEvent appEvent);
}