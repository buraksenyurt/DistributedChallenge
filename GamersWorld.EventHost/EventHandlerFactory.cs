using GamersWorld.SDK;
using Microsoft.Extensions.DependencyInjection;

namespace GamersWorld.EventHost.Factory;
public class EventHandlerFactory
{
    private readonly IServiceProvider _serviceProvider;

    public EventHandlerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IEventDriver<TEvent> CreateHandler<TEvent>()
        where TEvent : IEvent
    {
        return _serviceProvider.GetRequiredService<IEventDriver<TEvent>>();
    }

    public async Task ExecuteEvent<TEvent>(TEvent appEvent)
        where TEvent : IEvent
    {
        var handler = CreateHandler<TEvent>();
        await handler.Execute(appEvent);
    }
}