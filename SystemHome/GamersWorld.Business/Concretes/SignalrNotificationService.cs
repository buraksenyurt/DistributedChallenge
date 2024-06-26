using GamersWorld.Business.Contracts;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using SecretsAgent;

namespace GamersWorld.Business.Concretes
{
    public class SignalrNotificationService(ISecretStoreService secretStoreService, ILogger<SignalrNotificationService> logger) : INotificationService
    {
        private readonly ISecretStoreService _secretStoreService = secretStoreService;
        private readonly ILogger _logger = logger;

        public async Task PushAsync(string clientId, string message)
        {
            var hubAddress = await _secretStoreService.GetSecretAsync("HomeWebAppHubAddress");
            var hubConnection = new HubConnectionBuilder().WithUrl($"{hubAddress}/notifyHub").Build();
            await hubConnection.StartAsync();
            _logger.LogInformation("Pushing event to {clientId}", clientId);
            await hubConnection.InvokeAsync("NotifyClient", clientId, message);
        }
    }
}