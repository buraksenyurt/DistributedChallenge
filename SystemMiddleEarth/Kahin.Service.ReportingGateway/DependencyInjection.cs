using Kahin.Common.Validation;
using Kahin.MQ;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SecretsAgent;
using Steeltoe.Common.Http.Discovery;
using Steeltoe.Discovery.Client;

namespace Kahin.Service.ReportingGateway;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, ILogger logger)
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
        .AddRoundRobinLoadBalancer()
        .AddStandardResilienceHandler(options =>
        {
            // options.Retry.Delay = TimeSpan.FromSeconds(3);
            // options.Retry.MaxRetryAttempts = 5;
            options.Retry.OnRetry = async args =>
            {
                logger.LogWarning("Retry attempt #{AttemptNumber} due to: {Reason}",
                    args.AttemptNumber,
                    args.Outcome.Exception?.Message ?? args.Outcome.Result?.StatusCode.ToString());

                await ValueTask.CompletedTask;
            };
        });

        services.AddSingleton<IRedisService, RedisService>();

        return services;
    }
}