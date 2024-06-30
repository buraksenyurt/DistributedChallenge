using GamersWorld.Application.Contracts.Notification;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using SecretsAgent;

namespace GamersWorld.Application.Notification;

public class SignalrNotificationService(ISecretStoreService secretStoreService, ILogger<SignalrNotificationService> logger)
        : INotificationService
{
    private readonly ISecretStoreService _secretStoreService = secretStoreService;
    private readonly ILogger<SignalrNotificationService> _logger = logger;

    public async Task PushAsync(string message)
    {
        var hubAddress = await _secretStoreService.GetSecretAsync("HomeWebAppHubAddress");
        var hubConnection = new HubConnectionBuilder().WithUrl($"http://{hubAddress}").Build();
        hubConnection.StartAsync().Wait();
        await hubConnection.SendAsync("NotifyClient", message);
    }

    public async Task PushToUserAsync(string userId, string message)
    {
        var hubAddress = await _secretStoreService.GetSecretAsync("HomeWebAppHubAddress");
        var hubConnection = new HubConnectionBuilder().WithUrl($"http://{hubAddress}").Build();
        await hubConnection.StartAsync();
        _logger.LogInformation("Pushing event to specific user '{userId}'", userId);
        await hubConnection.InvokeAsync("NotifyEmployee", userId, message);
    }
}