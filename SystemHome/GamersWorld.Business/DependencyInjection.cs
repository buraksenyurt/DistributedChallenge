using GamersWorld.Business.Concretes;
using GamersWorld.Business.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace GamersWorld.Business;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessDrivers(this IServiceCollection services)
    {
        services.AddTransient<IDocumentSaver, FileSaver>();

        return services;
    }
}