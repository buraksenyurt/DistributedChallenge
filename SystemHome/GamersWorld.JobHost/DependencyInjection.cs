using GamersWorld.Application.Tasking;
using GamersWorld.Domain.Constants;
using GamersWorld.JobHost.Business;
using Microsoft.Extensions.DependencyInjection;

namespace GamersWorld.JobHost
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWorkers(this IServiceCollection services)
        {
            services.AddKeyedTransient<IJobAction, ReportArchiver>(Names.ReportArchiver);
            services.AddKeyedTransient<IJobAction, ReportEraser>(Names.ReportEraser);

            return services;
        }
    }
}
