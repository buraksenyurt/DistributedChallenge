using Microsoft.AspNetCore.SignalR;

namespace GamersWorld.WebApp;

public class EmployeeUserIdProvider
    : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.Identity?.Name ?? connection.GetHttpContext().Request.Query["employeeId"];
    }
}