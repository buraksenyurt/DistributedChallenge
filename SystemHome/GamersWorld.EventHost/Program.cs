using GamersWorld.EventHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using GamersWorld.Domain.Constants;
using SecretsAgent;
using GamersWorld.Application;
using GamersWorld.Repository;
using Steeltoe.Discovery.Client;
using Steeltoe.Common.Http.Discovery;
using Microsoft.Extensions.Http.Resilience;
using Polly;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", reloadOnChange: true, optional: false)
    .Build();

var services = new ServiceCollection();

// DI Servis eklemeleri
services.AddSingleton<IConfiguration>(configuration);
services.AddLogging(cfg =>
{
    cfg.AddConfiguration(configuration.GetSection("Logging"));
    cfg.AddConsole();
});
services.AddSingleton<ISecretStoreService, SecretStoreService>();

services.AddApplication();
services.AddEventDrivers();
services.AddData();
services.AddRabbitMq();
services.AddDiscoveryClient();
services.AddHttpClient(Names.KahinGateway, client =>
{
    client.BaseAddress = new Uri("http://reporting-gateway-service");
})
.AddServiceDiscovery()
.AddRoundRobinLoadBalancer()
.AddResilienceHandler("Resilience Pipeline",
    static builder =>
    {
        builder.AddRetry(new HttpRetryStrategyOptions
        {
            BackoffType = DelayBackoffType.Exponential,
            MaxRetryAttempts = 5,
            Delay = TimeSpan.FromSeconds(3),
            UseJitter = true
        });
    });

var serviceProvider = services.BuildServiceProvider();
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
var httpClient = httpClientFactory.CreateClient(Names.KahinGateway);
await ServiceController.IsReportingServiceAlive(httpClient, logger);

var eventConsumer = serviceProvider.GetService<EventConsumer>();

if (eventConsumer != null)
{
    logger.LogInformation("Event listener is online");
    eventConsumer.Run();
}
else
{
    logger.LogError("EventConsumer didn't started");
}
