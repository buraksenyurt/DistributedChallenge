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

    public EventConsumer(IConnectionFactory connectionFactory, IServiceProvider serviceProvider, ILogger<EventConsumer> logger)
    {
        _connectionFactory = connectionFactory;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public void Start()
    {
        using var connection = _connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: "report_events_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, args) =>
        {
            var body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var eventType = args.BasicProperties.Type;

            await HandleEvent(eventType, message);
        };

        channel.BasicConsume(queue: "report_events_queue", autoAck: true, consumer: consumer);

        Console.WriteLine("Kuyruk mesajları dinleniyor. Çıkmak için bir tuşa basın.");
        Console.ReadLine();
    }

    private async Task HandleEvent(string eventType, string message)
    {
        _logger.LogInformation($"Event: #{eventType} , Message: {message}");

        using var scope = _serviceProvider.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<EventHandlerFactory>();

        switch (eventType)
        {
            case nameof(ReportRequestedEvent):
                var reportRequestedEvent = JsonSerializer.Deserialize<ReportRequestedEvent>(message);
                await factory.ExecuteEvent(reportRequestedEvent);
                break;
            case nameof(ReportReadyEvent):
                var reportReadyEvent = JsonSerializer.Deserialize<ReportReadyEvent>(message);
                await factory.ExecuteEvent(reportReadyEvent);
                break;
            case nameof(ReportIsHereEvent):
                var reportIsHereEvent = JsonSerializer.Deserialize<ReportIsHereEvent>(message);
                await factory.ExecuteEvent(reportIsHereEvent);
                break;
            case nameof(ReportProcessCompletedEvent):
                var reportProcessCompletedEvent = JsonSerializer.Deserialize<ReportProcessCompletedEvent>(message);
                await factory.ExecuteEvent(reportProcessCompletedEvent);
                break;
            case nameof(InvalidExpressionEvent):
                var invalidExpressionEvent = JsonSerializer.Deserialize<InvalidExpressionEvent>(message);
                await factory.ExecuteEvent(invalidExpressionEvent);
                break;
            default:
                _logger.LogError("Event çözümlenemedi.");
                break;
        }
    }
}
