using GamersWorld.Application.Contracts.Data;
using Microsoft.Extensions.DependencyInjection;

namespace GamersWorld.Repository;

public static class DependencyInjection
{
    public static IServiceCollection AddData(this IServiceCollection services)
    {
        services.AddTransient<IReportDataRepository, ReportDataRepository>();
        services.AddTransient<IReportDocumentDataRepository, ReportDocumentDataRepository>();
        return services;
    }
}