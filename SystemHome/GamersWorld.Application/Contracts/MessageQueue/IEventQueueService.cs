namespace GamersWorld.Application.Contracts.MessageQueue;

public interface IEventQueueService
{
    void PublishEvent<T>(T eventMessage);
}