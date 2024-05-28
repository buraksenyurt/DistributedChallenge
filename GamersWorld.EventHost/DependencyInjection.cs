using GamersWorld.AppEventBusiness;
using GamersWorld.AppEvents;
using GamersWorld.SDK;
using Microsoft.Extensions.DependencyInjection;

namespace GamersWorld.EventHost;

public static class DependencyInjection
{
    public static IServiceCollection AddEventExecutors(this IServiceCollection services)
    {
        services.AddTransient<IEventExecuter<ReportRequestedEvent>, PostReportRequest>();
        services.AddTransient<IEventExecuter<ReportReadyEvent>, GetReportDocument>();
        services.AddTransient<IEventExecuter<ReportIsHereEvent>, UsePreparedReport>();
        services.AddTransient<IEventExecuter<ReportProcessCompletedEvent>, DeleteReport>();
        services.AddTransient<IEventExecuter<InvalidExpressionEvent>, InvalidExpression>();
        services.AddSingleton<EventExecuterFactory>();
        return services;
    }
}