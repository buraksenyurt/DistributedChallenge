using GamersWorld.Application.Contracts.Document;
using Microsoft.Extensions.DependencyInjection;

namespace GamersWorld.Repository;

public static class DependencyInjection
{
    public static IServiceCollection AddData(this IServiceCollection services)
    {
        services.AddTransient<IDocumentRepository, DocumentRepository>();
        return services;
    }
}