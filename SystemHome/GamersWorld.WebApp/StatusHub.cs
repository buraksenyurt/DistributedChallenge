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
            _logger.LogInformation("Client '{ClientId}' has been connected", clientId);
            _connections[Context.ConnectionId] = clientId;
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _connections.TryRemove(Context.ConnectionId, out var clientId);
        _logger.LogInformation("Client '{ClientId}' has been disconnected", clientId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task NotifyClient(string clientId, string message)
    {
        var connectionId = _connections.FirstOrDefault(x => x.Value == clientId).Key;
        if (connectionId != null)
        {
            _logger.LogWarning("Notification on '{ConnectionId}' with '{Message}'", connectionId, message);            
            await Clients.Client(connectionId).SendAsync("ReadNotification", message);
        }
    }
}