using GamersWorld.WebApp.Services;
using Microsoft.AspNetCore.SignalR;
using SecretsAgent;

namespace GamersWorld.WebApp;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services)
    {
        services.AddScoped<MessengerServiceClient>();
        services.AddSingleton<ISecretStoreService, SecretStoreService>();
        services.AddSingleton<IUserIdProvider, EmployeeUserIdProvider>();
        
        return services;
    }
}