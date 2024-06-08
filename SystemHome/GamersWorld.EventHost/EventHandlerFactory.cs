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

    // DI Service provider nesnesini kullanarak TEvent türünden gelen IEventDriver türetmesini örnekler
    public IEventDriver<TEvent> CreateHandler<TEvent>()
        where TEvent : IEvent
    {
        return _serviceProvider.GetRequiredService<IEventDriver<TEvent>>();
    }

    // Parametre olarak gelen Event için eşleştirilmiş Business fonksiyonelliğini çalıştırır

    public async Task ExecuteEvent<TEvent>(TEvent appEvent)
        where TEvent : IEvent
    {
        // TEvent türüne göre IEventDriver'ı yakala
        var handler = CreateHandler<TEvent>();
        // Yakalanan IEventDriver örneğinin Execute operasyonunu işlet
        await handler.Execute(appEvent);
    }
}