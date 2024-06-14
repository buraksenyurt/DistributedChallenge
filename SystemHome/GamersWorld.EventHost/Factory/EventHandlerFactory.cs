using GamersWorld.SDK;
using Microsoft.Extensions.DependencyInjection;

namespace GamersWorld.EventHost.Factory;
public class EventHandlerFactory(IServiceProvider serviceProvider)
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    public async Task ExecuteEvent<TEvent>(TEvent appEvent)
        where TEvent : IEvent
    {
        // TEvent türüne göre IEventDriver'ı yakala
        var handler = _serviceProvider.GetRequiredService<IEventDriver<TEvent>>();
        // Yakalanan IEventDriver örneğinin Execute operasyonunu işlet
        await handler.Execute(appEvent);
    }
}