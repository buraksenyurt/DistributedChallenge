namespace GamersWorld.MQ;

public interface IEventQueueService
{
    void PublishEvent<T>(T eventMessage);
}