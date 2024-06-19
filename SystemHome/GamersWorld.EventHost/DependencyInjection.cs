using GamersWorld.EventBusiness;
using GamersWorld.Events;
using GamersWorld.EventHost.Factory;
using GamersWorld.SDK;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using GamersWorld.Common.Constants;
using SecretsAgent;

namespace GamersWorld.EventHost;

public static class DependencyInjection
{
    // Event ve Business nesne bağımlılıklarının DI servislerine yükleyen metot

    public static IServiceCollection AddEventDrivers(this IServiceCollection services)
    {
        services.AddTransient<IEventDriver<ReportRequestedEvent>, PostReportRequest>();
        services.AddTransient<IEventDriver<ReportReadyEvent>, ReportDocumentAvailable>();
        services.AddTransient<IEventDriver<ReportIsHereEvent>, UsePreparedReport>();
        services.AddTransient<IEventDriver<ReportProcessCompletedEvent>, DeleteReport>();
        services.AddTransient<IEventDriver<InvalidExpressionEvent>, InvalidExpression>();
        services.AddSingleton<EventHandlerFactory>();

        return services;
    }

    // RabbitMq hizmetini DI servisine yükleyen fonksiyon
    public static IServiceCollection AddRabbitMq(this IServiceCollection services)
    {
        var secretStoreService = services.BuildServiceProvider().GetRequiredService<ISecretStoreService>();
        services.AddSingleton<IConnectionFactory>(c => new ConnectionFactory()
        {
            HostName = secretStoreService.GetSecretAsync(SecretName.RabbitMQHostName).GetAwaiter().GetResult(),
            UserName = secretStoreService.GetSecretAsync(SecretName.RabbitMQUsername).GetAwaiter().GetResult(),
            Password = secretStoreService.GetSecretAsync(SecretName.RabbitMQPassword).GetAwaiter().GetResult(),
            Port = Convert.ToInt32(secretStoreService.GetSecretAsync(SecretName.RabbitMQPort).GetAwaiter().GetResult())
        });
        services.AddSingleton<EventConsumer>();

        return services;
    }
}