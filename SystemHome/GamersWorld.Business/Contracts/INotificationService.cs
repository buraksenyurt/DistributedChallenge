namespace GamersWorld.Business.Contracts;

public interface INotificationService
{
    Task PushAsync(string message);
}