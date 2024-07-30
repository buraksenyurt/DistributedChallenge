using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace GamersWorld.WebApp;

[Authorize]
public class StatusHub : Hub
{
    public async Task NotifyClient(string message)
    {
        await Clients.All.SendAsync("ReadNotification", message);
    }

    public async Task NotifyEmployee(string employeeId, string message)
    {
        await Clients.User(employeeId).SendAsync("ReadNotification", message);
    }
}