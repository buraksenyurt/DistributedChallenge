using Kahin.Common.Validation;
using Kahin.MQ;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SecretsAgent;
using Steeltoe.Common.Http.Discovery;
using Steeltoe.Discovery.Client;

namespace Kahin.Service.ReportingGateway;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services)
    {
        services.AddSingleton<ISecretStoreService, SecretStoreService>();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());
        services.AddDiscoveryClient();
        services.AddHttpClient<ValidatorClient>(client =>
        {
            client.BaseAddress = new Uri("http://hal-audit-service");
        })
        .AddServiceDiscovery()
        .AddRoundRobinLoadBalancer();
        services.AddSingleton<IRedisService, RedisService>();

        return services;
    }
}