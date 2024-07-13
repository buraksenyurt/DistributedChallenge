using System.Text.Json;
using GamersWorld.EventHost.Factory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using GamersWorld.Domain.Constants;
using GamersWorld.Application.Contracts.Events;

namespace GamersWorld.EventHost;

public class EventConsumer(IConnectionFactory connectionFactory, IServiceProvider serviceProvider, ILogger<EventConsumer> logger)
{
    private readonly IConnectionFactory _connectionFactory = connectionFactory;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<EventConsumer> _logger = logger;

    public void Run()
    {
        using var connection = _connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(
            queue: Names.EventQueue,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += async (model, args) =>
        {
            var message = args.Body.ToArray();
            var eventType = args.BasicProperties.Type;
            await Handle(eventType, message);
        };

        channel.BasicConsume(queue: Names.EventQueue, autoAck: true, consumer: consumer);

        Console.WriteLine("Listening event queue...Press any key to exit.");
        Console.ReadLine();
    }

    private async Task Handle(string eventType, byte[] eventMessage)
    {
        _logger.LogInformation("Event: {EventType}, Message: {EventMessage}", eventType, eventMessage);

        using var scope = _serviceProvider.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<EventHandlerFactory>();

        switch (eventType)
        {
            case nameof(ReportRequestedEvent):
                var reportRequestedEvent = JsonSerializer.Deserialize<ReportRequestedEvent>(eventMessage);
                if (reportRequestedEvent != null)
                {
                    await factory.ExecuteEvent(reportRequestedEvent);
                }
                break;
            case nameof(ReportReadyEvent):
                var reportReadyEvent = JsonSerializer.Deserialize<ReportReadyEvent>(eventMessage);
                if (reportReadyEvent != null)
                {
                    await factory.ExecuteEvent(reportReadyEvent);
                }
                break;
            case nameof(ReportIsHereEvent):
                var reportIsHereEvent = JsonSerializer.Deserialize<ReportIsHereEvent>(eventMessage);
                if (reportIsHereEvent != null)
                {
                    await factory.ExecuteEvent(reportIsHereEvent);
                }
                break;
            case nameof(ReportProcessCompletedEvent):
                var reportProcessCompletedEvent = JsonSerializer.Deserialize<ReportProcessCompletedEvent>(eventMessage);
                if (reportProcessCompletedEvent != null)
                {
                    await factory.ExecuteEvent(reportProcessCompletedEvent);
                }
                break;
            case nameof(InvalidExpressionEvent):
                var invalidExpressionEvent = JsonSerializer.Deserialize<InvalidExpressionEvent>(eventMessage);
                if (invalidExpressionEvent != null)
                {
                    await factory.ExecuteEvent(invalidExpressionEvent);
                }
                break;
            case nameof(ArchiveReportEvent):
                var archiveReportEvent = JsonSerializer.Deserialize<ArchiveReportEvent>(eventMessage);
                if (archiveReportEvent != null)
                {
                    await factory.ExecuteEvent(archiveReportEvent);
                }
                break;
            default:
                _logger.LogError("Undefined Event");
                break;
        }
    }
}