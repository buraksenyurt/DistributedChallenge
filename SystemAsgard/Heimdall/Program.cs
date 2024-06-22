using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Heimdall.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHealthChecks()
    .AddRedis(
        redisConnectionString: "localhost:6379"
        , name: "Redis"
        , tags: ["Docker-Compose", "Redis"])
    .AddRabbitMQ(
        rabbitConnectionString: "amqp://scothtiger:123456@localhost:5672/"
        , name: "RabbitMQ"
        , tags: ["Docker-Compose", "Rabbit MQ"]
        )
    .AddCheck(
        name: "Eval Audit Api"
        , instance: new HealthChecker(new Uri("http://localhost:5147/health"))
        , tags: ["SystemHAL", "REST"]
    )
    .AddCheck(
        name: "GamersWorld Gateway"
        , instance: new HealthChecker(new Uri("http://localhost:5102/health"))
        , tags: ["SystemHOME", "REST"]
    )
    .AddCheck(
        name: "GamersWorld Messenger"
        , instance: new HealthChecker(new Uri("http://localhost:5234/health"))
        , tags: ["SystemHOME", "REST", "BackendApi"]
    )
    .AddCheck(
        name: "GamersWorld Web App"
        , instance: new HealthChecker(new Uri("http://localhost:5093/health"))
        , tags: ["SystemHOME", "WebApp"]
    )
    .AddCheck(
        name: "Kahin Reporting Gateway"
        , instance: new HealthChecker(new Uri("http://localhost:5218/health"))
        , tags: ["SystemMIDDLE_EARTH", "REST"]
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
