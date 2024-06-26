namespace GamersWorld.Business.Contracts;

public interface INotificationService
{
    Task PushAsync(string message);
    Task PushToUserAsync(string userId, string message);
}