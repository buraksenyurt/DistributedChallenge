using Kahin.Common.Services;
using Kahin.EventHost;
using Kahin.MQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<ISecretStoreService, SecretStoreService>();
        services.AddSingleton<IRedisService, RedisService>();
        services.AddHostedService<Worker>();
        services.AddHttpClient<IHomeGatewayClientService, HomeGatewayClientService>();
    })
    .Build();

Console.WriteLine("Press any key to start Host App");
Console.ReadLine();

await host.RunAsync();