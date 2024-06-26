using GamersWorld.Business.Contracts;
using Microsoft.AspNetCore.SignalR.Client;
using SecretsAgent;

namespace GamersWorld.Business.Concretes;

public class SignalrNotificationService(ISecretStoreService secretStoreService)
        : INotificationService
{
    private readonly ISecretStoreService _secretStoreService = secretStoreService;

    public async Task PushAsync(string clientId, string message)
    {
        var hubAddress = await _secretStoreService.GetSecretAsync("HomeWebAppHubAddress");
        HubConnection hubConnection = new HubConnectionBuilder().WithUrl($"http://{hubAddress}").Build();
        hubConnection.StartAsync().Wait();
        await hubConnection.SendAsync("NotifyClient", clientId, message);
    }
}