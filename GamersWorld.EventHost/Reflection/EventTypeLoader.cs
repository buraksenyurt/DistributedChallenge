namespace GamersWorld.EventHost.Reflection;

public static class EventTypeLoader
{
    public static Type ReflectionLoad(string eventType) =>
        Type.GetType(
            $"{nameof(GamersWorld)}.{nameof(AppEvents)}.{eventType}, {nameof(GamersWorld)}.{nameof(AppEvents)}"
        );
}