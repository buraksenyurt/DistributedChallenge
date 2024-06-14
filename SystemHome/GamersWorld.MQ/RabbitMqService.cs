using GamersWorld.Common.Settings;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text.Json;

namespace GamersWorld.MQ;

/*
    RabbitMq tarafına mesaj göndermek için kullanılan servis sınıfı
*/
public class RabbitMqService
    : IEventQueueService, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqService(IConfiguration configuration)
    {
        var settings = configuration.GetSection("RabbitMqSettings").Get<RabbitMqSettings>();
        var factory = new ConnectionFactory();
        if (settings == null)
        {
            factory.HostName = "localhost";
            factory.UserName = "scothtiger";
            factory.Password = "P@ssw0rd";
            factory.Port = 5672;
        }
        else
        {
            factory.HostName = settings.HostName;
            factory.UserName = settings.Username;
            factory.Password = settings.Password;
            factory.Port = settings.Port;
        }

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public void PublishEvent<T>(T eventMessage)
    {
        var queueName = "report_events_queue";
        _channel.QueueDeclare(queue: queueName,
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var body = JsonSerializer.SerializeToUtf8Bytes(eventMessage);

        var properties = _channel.CreateBasicProperties();
        properties.Type = typeof(T).Name;

        _channel.BasicPublish(exchange: "",
                             routingKey: queueName,
                             basicProperties: properties,
                             body: body);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        _channel?.Close();
        _connection?.Close();
    }
}
