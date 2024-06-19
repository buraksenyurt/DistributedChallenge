using GamersWorld.Common.Constants;
using Kahin.Common.Services;
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

    public RabbitMqService(ISecretStoreService secretStoreService)
    {
        var factory = new ConnectionFactory
        {
            HostName = secretStoreService.GetSecretAsync(SecretName.RabbitMQHostName).GetAwaiter().GetResult(),
            UserName = secretStoreService.GetSecretAsync(SecretName.RabbitMQUsername).GetAwaiter().GetResult(),
            Password = secretStoreService.GetSecretAsync(SecretName.RabbitMQPassword).GetAwaiter().GetResult(),
            Port = Convert.ToInt32(secretStoreService.GetSecretAsync(SecretName.RabbitMQPort).GetAwaiter().GetResult())
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public void PublishEvent<T>(T eventMessage)
    {
        var queueName = Names.EventQueue;
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
