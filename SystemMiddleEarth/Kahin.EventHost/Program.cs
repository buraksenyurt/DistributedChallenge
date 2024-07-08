using Kahin.Common.Services;
using Kahin.EventHost;
using Kahin.MQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        services.AddHttpClient<HomeGatewayServiceClient>(client =>
        {
            client.BaseAddress = new Uri("http://home-gateway-service");
        })
        .AddServiceDiscovery()
        .AddRoundRobinLoadBalancer();
        services.AddSingleton(context.Configuration);
    })
    .Build();

Console.WriteLine("Press any key to start Host App");
Console.ReadLine();

await host.RunAsync();