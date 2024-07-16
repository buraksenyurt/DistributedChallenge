using Kahin.Common.Services;
using Kahin.EventHost;
using Kahin.MQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Polly;
using SecretsAgent;
using Steeltoe.Common.Http.Discovery;
using Steeltoe.Discovery.Client;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<ISecretStoreService, SecretStoreService>();
        services.AddSingleton<IRedisService, RedisService>();
        services.AddHostedService<Worker>();
        services.AddDiscoveryClient();
        services.AddHttpClient<HomeGatewayServiceClient>("HomeGatewayService", client =>
        {
            client.BaseAddress = new Uri("http://home-gateway-service");
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

        services.AddSingleton(context.Configuration);
    })
    .Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();

var httpClientFactory = host.Services.GetRequiredService<IHttpClientFactory>();
var httpClient = httpClientFactory.CreateClient("HomeGatewayService");
await ServiceController.IsReportingServiceAlive(httpClient, logger);

//Console.WriteLine("Press any key to start Host App");
//Console.ReadLine();

await host.RunAsync();