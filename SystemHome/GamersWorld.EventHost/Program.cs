using GamersWorld.EventHost;
using GamersWorld.Business;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Kahin.Common.Services;
using GamersWorld.Common.Constants;

// RabbitMq ayarlarını da ele alacağımız için appSettings konfigurasyonu için bir builder nesnesi örnekledik
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

// Olay sürücüleri, RabbitMq ve Loglama gibi bileşenler DI servislerine yüklenir
services.AddEventDrivers();
services.AddBusinessDrivers();
services.AddRabbitMq();
services.AddHttpClient(Names.KahinGateway, (serviceProvider, client) =>
{
    var secretStoreService = serviceProvider.GetRequiredService<ISecretStoreService>();
    var reportingServiceHostAddress = secretStoreService.GetSecretAsync(SecretName.KahinReportingGatewayApiAddres).GetAwaiter().GetResult();
    client.BaseAddress = new Uri($"http://{reportingServiceHostAddress}");
});

var serviceProvider = services.BuildServiceProvider();
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
var httpClient = httpClientFactory.CreateClient(Names.KahinGateway);
if (await ServiceController.IsReportingServiceAlive(httpClient, logger))
{
    logger.LogInformation("Reporting service is up and online.");
}
else
{
    logger.LogError("Reporting service isn't working!");
}

// Mesaj kuyruğunu dinleyecek nesne örneklenir
var eventConsumer = serviceProvider.GetService<EventConsumer>();

// Dinleme fonksiyonu başlatılır
if (eventConsumer != null)
{
    logger.LogInformation("Event listener is online");
    eventConsumer.Run();
}
else
{
    logger.LogError("EventConsumer didn't started");
}
