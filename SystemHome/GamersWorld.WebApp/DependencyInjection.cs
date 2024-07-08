using GamersWorld.WebApp.Services;
using Microsoft.AspNetCore.SignalR;

namespace GamersWorld.WebApp;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services)
    {
        services.AddScoped<MessengerServiceClient>();
        services.AddSingleton<IUserIdProvider, EmployeeUserIdProvider>();
        
        return services;
    }
}