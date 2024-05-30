using GamersWorld.AppEventBusiness;
using GamersWorld.AppEvents;
using GamersWorld.Common.Settings;
using GamersWorld.EventHost.Factory;
using GamersWorld.SDK;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace GamersWorld.EventHost;

public static class DependencyInjection
{    
    // Event ve Business nesne bağımlılıklarının DI servislerine yükleyen metot

    public static IServiceCollection AddEventDrivers(this IServiceCollection services)
    {
        services.AddTransient<IEventDriver<ReportRequestedEvent>, PostReportRequest>();
        services.AddTransient<IEventDriver<ReportReadyEvent>, GetReportDocument>();
        services.AddTransient<IEventDriver<ReportIsHereEvent>, UsePreparedReport>();
        services.AddTransient<IEventDriver<ReportProcessCompletedEvent>, DeleteReport>();
        services.AddTransient<IEventDriver<InvalidExpressionEvent>, InvalidExpression>();
        services.AddSingleton<EventHandlerFactory>();
        return services;
    }

    // RabbitMq hizmetini DI servisine yükleyen fonksiyon
    
    public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqSettings = new RabbitMqSettings();
        configuration.GetSection("RabbitMqSettings").Bind(rabbitMqSettings);

        // RabbitMQ bağlantı bilgileri appSettings sekmesinden çekilir
        services.AddSingleton<IConnectionFactory>(c => new ConnectionFactory()
        {
            HostName = rabbitMqSettings.HostName,
            UserName = rabbitMqSettings.Username,
            Password = rabbitMqSettings.Password,
            Port = rabbitMqSettings.Port
        });
        services.AddSingleton<EventConsumer>();
        return services;
    }
}