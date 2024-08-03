using GamersWorld.Application.Contracts.Data;
using GamersWorld.Application.Contracts.Notification;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using SecretsAgent;

namespace GamersWorld.Application.Notification;

public class SignalrNotificationService(ISecretStoreService secretStoreService, IEmployeeTokenDataRepository repository, ILogger<SignalrNotificationService> logger)
        : INotificationService
{
    private readonly ISecretStoreService _secretStoreService = secretStoreService;
    private readonly ILogger<SignalrNotificationService> _logger = logger;
    private readonly IEmployeeTokenDataRepository _repository = repository;

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
        var token = await _repository.ReadToken(userId);

        var hubConnection = new HubConnectionBuilder()
            .WithUrl($"http://{hubAddress}", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token);
            })
            .Build();

        await hubConnection.StartAsync();
        _logger.LogInformation("Pushing event to specific user '{userId}'", userId);
        await hubConnection.InvokeAsync("NotifyEmployee", userId, message);
    }
}