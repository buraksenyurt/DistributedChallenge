using Kahin.Common.Services;
using Kahin.EventHost;
using Kahin.MQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Polly;
using SecretsAgent;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Steeltoe.Common.Http.Discovery;
using Steeltoe.Discovery.Client;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
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

        var serviceProvider = services.BuildServiceProvider();
        var secretStoreService = serviceProvider.GetRequiredService<ISecretStoreService>();
        var elasticsearchAddress = $"http://{secretStoreService.GetSecretAsync("ElasticsearchAddress").GetAwaiter().GetResult()}";
        var environment = context.Configuration["Environment"] ?? "Development";
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("System", "KahinEventHost")
            .Enrich.WithProperty("Environment", environment)
            .WriteTo.Console()
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticsearchAddress))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"kahin-event-host-logs-{environment.ToLower()}",
                TypeName = null,
                BatchAction = ElasticOpType.Create,
                ModifyConnectionSettings = x => x.ServerCertificateValidationCallback((sender, cert, chain, errors) => true)
            })
            .CreateLogger();
    })
    .UseSerilog()
    .Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();

var httpClientFactory = host.Services.GetRequiredService<IHttpClientFactory>();
var httpClient = httpClientFactory.CreateClient("HomeGatewayService");
await ServiceController.IsReportingServiceAlive(httpClient, logger);

await host.RunAsync();
