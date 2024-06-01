using System.Text;
using System.Text.Json;
using GamersWorld.AppEvents;
using GamersWorld.EventHost.Factory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GamersWorld.EventHost;

public class EventConsumer
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventConsumer> _logger;

    public EventConsumer(IConnectionFactory connectionFactory, IServiceProvider serviceProvider,
        ILogger<EventConsumer> logger)
    {
        _connectionFactory = connectionFactory;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public void Run()
    {
        // RabitMq bağlantısı tesis edilir ve bir kanal oluşturulur
        using var connection = _connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();
        // reports_event_queue isimli bir kuyruk tanımlanır
        channel.QueueDeclare(queue: "report_events_queue", durable: false, exclusive: false, autoDelete: false,
            arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        // Gelen mesajların yakalandığı olay metodu
        // Lambda operatörü üzerinden anonymous function olarak event handler temsilcisini bağlanır
        consumer.Received += async (model, args) =>
        {
            var message = args.Body.ToArray();
            var eventType =
                args.BasicProperties.Type; // Publish edilecek mesajı type property değerinden yakalayabiliriz

            // Kuyruktan yakalanan mesaj değerlendirilmek üzere Handle operasyonuna gönderilir
            await Handle(eventType, message);
        };

        channel.BasicConsume(queue: "report_events_queue", autoAck: true, consumer: consumer);

        Console.WriteLine("Kuyruk mesajları dinleniyor. Çıkmak için bir tuşa basın.");
        Console.ReadLine();
    }

    private async Task Handle(string eventType, byte[] eventMessage)
    {
        // _logger.LogInformation("Event: #{} , Message: {}", eventType, eventMessage);

        using var scope = _serviceProvider.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<EventHandlerFactory>();

        // Kuyruktan yakalanan Event ve mesaj içeriği burada değerlendirlir
        // eventType türüne göre JSON formatından döndürülen mesaj içeriği
        // factory nesnesi üzerinden uygun business nesnesinin execute fonksiyonuna kadar gönderilir
        //TODO@buraksenyurt Yeni Event-Business Object eşleşmeleri geldikçe buradaki switch bloğu büyümeye devam edecek.
        // Belki bir Dictionary ve Reflection ile konfigurasyon dosyası gibi bir yerden bu execution işini yönetebiliriz.

        switch (eventType)
        {
            case nameof(ReportRequestedEvent):
                var reportRequestedEvent = JsonSerializer.Deserialize<ReportRequestedEvent>(eventMessage);
                await factory.ExecuteEvent(reportRequestedEvent);
                break;
            case nameof(ReportReadyEvent):
                var reportReadyEvent = JsonSerializer.Deserialize<ReportReadyEvent>(eventMessage);
                await factory.ExecuteEvent(reportReadyEvent);
                break;
            case nameof(ReportIsHereEvent):
                var reportIsHereEvent = JsonSerializer.Deserialize<ReportIsHereEvent>(eventMessage);
                await factory.ExecuteEvent(reportIsHereEvent);
                break;
            case nameof(ReportProcessCompletedEvent):
                var reportProcessCompletedEvent = JsonSerializer.Deserialize<ReportProcessCompletedEvent>(eventMessage);
                await factory.ExecuteEvent(reportProcessCompletedEvent);
                break;
            case nameof(InvalidExpressionEvent):
                var invalidExpressionEvent = JsonSerializer.Deserialize<InvalidExpressionEvent>(eventMessage);
                await factory.ExecuteEvent(invalidExpressionEvent);
                break;
            default:
                _logger.LogError("Event çözümlenemedi.");
                break;
        }
    }
}