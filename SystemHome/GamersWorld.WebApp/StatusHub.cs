using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace GamersWorld.WebApp;

[Authorize]
public class StatusHub(ILogger<StatusHub> logger) : Hub
{
    private readonly ILogger<StatusHub> _logger = logger;
    public async Task NotifyClient(string message)
    {
        await Clients.All.SendAsync("ReadNotification", message);
    }

    public async Task NotifyEmployee(string employeeId, string message)
    {
        _logger.LogWarning("Triggered NotifyEmployee method for {EmployeeId}", employeeId);
        await Clients.User(employeeId).SendAsync("ReadNotification", message);
    }
}