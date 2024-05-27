using GamersWorld.SDK;
using Microsoft.Extensions.DependencyInjection;

namespace GamersWorld.EventHost
{
    public class EventExecuterFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public EventExecuterFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void ExecuteEvent<TEvent>(TEvent appEvent)
            where TEvent : IEvent
        {
            var executer = _serviceProvider.GetService<IEventExecuter<TEvent>>();
            executer?.Execute(appEvent);
        }
    }
}
