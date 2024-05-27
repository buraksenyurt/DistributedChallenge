namespace GamersWorld.SDK;

public interface IEventExecuter
{
    void Execute(IEvent appEvent);
}