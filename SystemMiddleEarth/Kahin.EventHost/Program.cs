using Kahin.Common.Services;
using Kahin.EventHost;
using Kahin.MQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        var redisConfig = context.Configuration.GetSection("Redis");
        services.AddSingleton<IRedisService>(new RedisService(redisConfig["ConnectionString"] ?? "localhost:6379"));
        services.AddHostedService<Worker>();
        services.AddHttpClient<IHomeGatewayClientService, HomeGatewayClientService>();
    })
    .Build();

Console.WriteLine("Press any key to start Host App");
Console.ReadLine();

await host.RunAsync();