using GamersWorld.Business.Concretes;
using GamersWorld.Business.Contracts;
using GamersWorld.MQ;
using GamersWorld.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace GamersWorld.Business;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessDrivers(this IServiceCollection services)
    {
        services.AddTransient<IDocumentRepository, DocumentRepository>();
        services.AddTransient<IDocumentWriter, TableSaver>();
        services.AddTransient<IDocumentReader, TableReader>();
        //services.AddTransient<IDocumentWriter, FileSaver>();
        services.AddTransient<IEventQueueService, RabbitMqService>();

        return services;
    }
}