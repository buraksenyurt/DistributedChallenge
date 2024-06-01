using GamersWorld.EventHost.Factory;

namespace GamersWorld.EventHost.Reflection;

public static class EventHandlerFactoryExtensions
{
    public static Task ReflectionInvoke(this EventHandlerFactory factory, Type type, object obj) =>
        (Task)factory
            .GetType()
            .GetMethod(nameof(EventHandlerFactory.ExecuteEvent))
            .MakeGenericMethod(type)
            .Invoke(factory, [obj]);
}