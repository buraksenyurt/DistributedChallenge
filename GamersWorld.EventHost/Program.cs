using GamersWorld.EventHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// RabbitMq ayarlarını da ele alacağımız için appSettings konfigurasyonu için bir builder nesnesi örnekledik
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", reloadOnChange: true, optional: false)
    .Build();

var services = new ServiceCollection();

// DI Servis eklemeleri
services.AddSingleton<IConfiguration>(configuration);

// Olay sürücüleri, RabbitMq ve Loglama gibi bileşenler DI servislerine yüklenir
services.AddEventDrivers();
services.AddRabbitMq(configuration);
services.AddLogging(cfg =>
{
    cfg.AddConfiguration(configuration.GetSection("Logging"));
    cfg.AddConsole();
});
services.AddHttpClient("KahinGateway", client =>
{
    var reportingServiceHostAddress = configuration["Kahin:ReportingService_HostAddress"];
    client.BaseAddress = new Uri(reportingServiceHostAddress ?? "http://localhost:5218");
});

var serviceProvider = services.BuildServiceProvider();
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
var httpClient = httpClientFactory.CreateClient("KahinGateway");
if (await ServiceController.IsReportingServiceAlive(httpClient, logger))
{
    logger.LogInformation("Reporting service ulaşılabilir durumda.");
}
else
{
    logger.LogError("Reporting service çalışmıyor görünüyor. Event dinleyici etkinleşebilir ama rapor iletimlerinde sorun olacaktır.");
}

// Mesaj kuyruğunu dinleyecek nesne örneklenir
var eventConsumer = serviceProvider.GetService<EventConsumer>();

// Dinleme fonksiyonu başlatılır
if (eventConsumer != null)
{
    logger.LogInformation("Event dinleneyici aktif");
    eventConsumer.Run();
}
else
{
    logger.LogError("EventConsumer başlatılamadı");
}