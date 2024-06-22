using Kahin.Common.Constants;
using Kahin.Common.Validation;
using Kahin.MQ;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SecretsAgent;

namespace Kahin.ReportingGateway;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services)
    {
        services.AddSingleton<ISecretStoreService, SecretStoreService>();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());
        services.AddHttpClient(Names.EvalApi, (serviceProvider, client) =>
        {
            var secretsService = serviceProvider.GetRequiredService<ISecretStoreService>();
            var evalApiServiceAddress = secretsService
                .GetSecretAsync(SecretName.EvalServiceApiAddress)
                .GetAwaiter()
                .GetResult();
            client.BaseAddress = new Uri($"http://{evalApiServiceAddress}/api");
        });
        services.AddTransient<ValidatorClient>();
        services.AddSingleton<IRedisService, RedisService>();
        return services;
    }
}