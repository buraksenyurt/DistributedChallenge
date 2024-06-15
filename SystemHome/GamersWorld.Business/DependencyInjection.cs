using GamersWorld.Business.Concretes;
using GamersWorld.Business.Contracts;
using GamersWorld.MQ;
using Microsoft.Extensions.DependencyInjection;

namespace GamersWorld.Business;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessDrivers(this IServiceCollection services)
    {
        services.AddTransient<IDocumentSaver, FileSaver>();
        services.AddTransient<IEventQueueService, RabbitMqService>();

        return services;
    }
}