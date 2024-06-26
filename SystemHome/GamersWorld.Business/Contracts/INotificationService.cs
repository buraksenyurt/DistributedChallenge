namespace GamersWorld.Business.Contracts;

public interface INotificationService
{
    Task PushAsync(string clientId, string message);
}