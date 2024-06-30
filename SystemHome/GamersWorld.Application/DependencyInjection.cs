using GamersWorld.Application.Contracts.Document;
using GamersWorld.Application.Contracts.MessageQueue;
using GamersWorld.Application.Contracts.Notification;
using GamersWorld.Application.Document;
using GamersWorld.Application.MessageQueue;
using GamersWorld.Application.Notification;
using Microsoft.Extensions.DependencyInjection;

namespace GamersWorld.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<INotificationService, SignalrNotificationService>();
        services.AddTransient<IDocumentWriter, TableSaver>();
        services.AddTransient<IDocumentReader, TableReader>();
        //services.AddTransient<IDocumentWriter, FileSaver>();
        services.AddTransient<IEventQueueService, RabbitMqService>();

        return services;
    }
}