namespace GamersWorld.Application.Contracts.Notification;

public interface INotificationService
{
    Task PushAsync(string message);
    Task PushToUserAsync(string userId, string message);
}