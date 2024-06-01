using GamersWorld.Common.Settings;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace GamersWorld.MQ;

/*
    RabbitMq tarafına mesaj göndermek için kullanılan servis sınıfı
*/
public class RabbitMqService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqService(RabbitMqSettings settings)
    {
        var factory = new ConnectionFactory()
        {
            HostName = settings.HostName,
            UserName = settings.Username,
            Password = settings.Password,
            Port = settings.Port
        };

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
        _channel?.Close();
        _connection?.Close();
    }
}
