using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace GamersWorld.WebApp;

public class StatusHub(ILogger<StatusHub> logger)
    : Hub
{
    private readonly ILogger<StatusHub> _logger = logger;
    private static readonly ConcurrentDictionary<string, string> _connections = new();

    public override async Task OnConnectedAsync()
    {
        var context = Context.GetHttpContext();
        if (context != null && context.Request != null)
        {
            var clientId = context.Request.Query["clientId"].ToString();
            _logger.LogInformation("{ClientId} has been connected", clientId);
            _connections[Context.ConnectionId] = clientId;
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _connections.TryRemove(Context.ConnectionId, out var clientId);
        _logger.LogInformation("{ClientId} has been connected", clientId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task NotifyClient(string clientId, string message)
    {
        await Clients.Client(clientId).SendAsync("ReadNotification", message);
    }
}