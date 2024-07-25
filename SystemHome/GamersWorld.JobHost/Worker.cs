using GamersWorld.Application.Tasking;
using GamersWorld.JobHost.Model;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GamersWorld.JobHost;

internal class Worker(
    IOptions<JobHeader> jobHeader
    , ILogger<Worker> logger
    , IRecurringJobManager recurringJobManager
    , IServiceProvider serviceProvider
    )
{
    private readonly JobHeader _jobHeader = jobHeader.Value;
    private readonly ILogger<Worker> _logger = logger;
    private readonly IRecurringJobManager _recurringJobManager = recurringJobManager;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task ExecuteJobs()
    {
        foreach (var job in _jobHeader.Jobs.Where(j => j.Active))
        {
            _logger.LogInformation("{Job Action}.'{Description}' has been scheduled.", job.ActionType, job.Description);
            _recurringJobManager.AddOrUpdate(job.Name, () => ExecuteJob(job.ActionType), job.CronExpression);
        }

        await Task.CompletedTask;
    }

    public void ExecuteJob(string actionType)
    {
        var service = _serviceProvider.GetRequiredKeyedService<IJobAction>(actionType);
        service.Execute();
    }
}