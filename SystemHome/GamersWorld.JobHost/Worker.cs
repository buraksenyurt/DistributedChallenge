using GamersWorld.JobHost.Model;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GamersWorld.JobHost;
internal class Worker(IOptions<JobHeader> jobHeader, ILogger<Worker> logger, IRecurringJobManager recurringJobManager)
{
    private readonly JobHeader _jobHeader = jobHeader.Value;
    private readonly ILogger<Worker> _logger = logger;
    private readonly IRecurringJobManager _recurringJobManager = recurringJobManager;

    public async Task ExecuteJobs()
    {
        foreach (var job in _jobHeader.Jobs)
        {
            _logger.LogInformation("{Job Action}.'{Description}' has been scheduled.", job.ActionName, job.Description);
            _recurringJobManager.AddOrUpdate(job.Name, () => ExecuteJob(job.ActionName), job.CronExpression);
        }

        await Task.CompletedTask;
    }

    public void ExecuteJob(string actionName)
    {
        var method = typeof(Worker).GetMethod(actionName);
        method?.Invoke(this, null);
    }
    public void TruncateDeadReports()
    {
        _logger.LogInformation("Truncate Dead Reports started at: {ExecuteTime}", DateTime.Now);
    }
}

