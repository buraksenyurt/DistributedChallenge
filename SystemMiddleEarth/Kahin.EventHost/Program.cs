using Kahin.Common.Services;
using Kahin.EventHost;
using Kahin.MQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

SecretStoreService secretsService = new();
string redisConnectionString = await secretsService.GetSecretAsync("RedisConnectionString");

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<IRedisService>(new RedisService(redisConnectionString));
        services.AddSingleton<ISecretStoreService>(new SecretStoreService());
        services.AddHostedService<Worker>();
        services.AddHttpClient<IHomeGatewayClientService, HomeGatewayClientService>();
    })
    .Build();

Console.WriteLine("Press any key to start Host App");
Console.ReadLine();

await host.RunAsync();