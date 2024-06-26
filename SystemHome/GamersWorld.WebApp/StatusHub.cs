using Microsoft.AspNetCore.SignalR;

namespace GamersWorld.WebApp;

public class StatusHub : Hub
{
    public async Task NotifyClient(string message)
    {
        await Clients.All.SendAsync("ReadNotification", message);
    }
}