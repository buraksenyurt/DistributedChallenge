using GamersWorld.AppEventBusiness;
using GamersWorld.EventHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// RabbitMq ayarlarını da ele alacağımız için appSettings konfigurasyonu için bir builder nesnesi örnekledik
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", reloadOnChange: true, optional: false)
    .Build();

var reportingServiceHostAddress = configuration["Kahin:ReportingService_HostAddress"];
if (string.IsNullOrEmpty(reportingServiceHostAddress))
{
    throw new ArgumentNullException("Kahin:ReportingService_HostAddress", "Kahin servis adresi bulunamadı.");
}
Console.WriteLine("{0}", reportingServiceHostAddress);

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
    client.BaseAddress = new Uri(reportingServiceHostAddress);
});

var serviceProvider = services.BuildServiceProvider();

// Get the logger
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Kahin Gateway Address: {HostAddress}", reportingServiceHostAddress);

// Mesaj kuyruğunu dinleyecek nesne örneklenir
var eventConsumer = serviceProvider.GetService<EventConsumer>();

// Dinleme fonksiyonu başlatılır
eventConsumer.Run();