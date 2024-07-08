using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Heimdall.Services;
using SecretsAgent;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var loggerFactory = LoggerFactory.Create(logging =>
{
    logging.AddConsole();
});
ILogger<SecretStoreService> logger = loggerFactory.CreateLogger<SecretStoreService>();
IConfiguration configuration = builder.Configuration;
var secretStoreService = new SecretStoreService(logger, configuration);

builder.Services.AddHealthChecks()
    .AddRedis(
        redisConnectionString: secretStoreService.GetSecretAsync("RedisConnectionString")
                    .GetAwaiter().GetResult(),
        name: "Redis",
        tags: ["Docker-Compose", "Redis"])
    .AddRabbitMQ(
        rabbitConnectionString: secretStoreService.GetSecretAsync("RabbitAmqpConnectionString")
                    .GetAwaiter().GetResult(),
        name: "RabbitMQ",
        tags: ["Docker-Compose", "RabbitMQ"])
    .AddNpgSql(connectionString: secretStoreService.GetSecretAsync("ReportDbConnStr")
                    .GetAwaiter().GetResult(),
        name: "Report Db",
        tags: ["Docker-Compose", "PostgreSQL", "Database"]
    )
    .AddConsul(setup =>
        {
            setup.HostName = "localhost";
            setup.Port = 8500;
            setup.RequireHttps = false;
        },
        name: "Consul",
        tags: ["Docker-Compose", "Consul", "Service-Discovery", "hashicorp"]
        )
    .AddCheck(
        name: "Eval Audit Api",
        instance: new HealthChecker(
                    new Uri($"http://{secretStoreService.GetSecretAsync("EvalServiceApiAddress")
                    .GetAwaiter().GetResult()}/health")),
        tags: ["SystemHAL", "REST"]
    )
    .AddCheck(
        name: "GamersWorld Gateway",
        instance: new HealthChecker(
                    new Uri($"http://{secretStoreService.GetSecretAsync("HomeGatewayApiAddress")
                    .GetAwaiter().GetResult()}/health")),
        tags: ["SystemHOME", "REST"]
    )
    .AddCheck(
        name: "GamersWorld Messenger",
        instance: new HealthChecker(
                    new Uri($"http://{secretStoreService.GetSecretAsync("MessengerApiAddress")
                    .GetAwaiter().GetResult()}/health")),
        tags: ["SystemHOME", "REST", "BackendApi"]
    )
    .AddCheck(
        name: "GamersWorld Web App",
        instance: new HealthChecker(
                    new Uri($"http://{secretStoreService.GetSecretAsync("HomeWebAppAddress")
                    .GetAwaiter().GetResult()}/health")),
        tags: ["SystemHOME", "WebApp"]
    )
    .AddCheck(
        name: "Kahin Reporting Gateway",
        instance: new HealthChecker(
                    new Uri($"http://{secretStoreService.GetSecretAsync("KahinReportingGatewayApiAddress")
                    .GetAwaiter().GetResult()}/health")),
        tags: ["SystemMIDDLE_EARTH", "REST"]
    );

builder.Services.AddHealthChecksUI(setupSettings =>
{
    setupSettings.SetHeaderText("Inventory Health Check Gate");
    setupSettings.AddHealthCheckEndpoint("Basic Health Check", "/health");
    setupSettings.SetEvaluationTimeInSeconds(10);
    setupSettings.SetApiMaxActiveRequests(2);
}).AddInMemoryStorage();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHealthChecksUI(config => config.UIPath = "/health-ui");

app.Run();
