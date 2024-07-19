using GamersWorld.Application.Contracts.Document;
using GamersWorld.Domain.Requests;
using GamersWorld.JobHost.Model;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GamersWorld.JobHost;

//TODO@burak Need to refactor because of dependent objects count increases for every new job business

internal class Worker(
    IOptions<JobHeader> jobHeader
    , ILogger<Worker> logger
    , IRecurringJobManager recurringJobManager
    , IDocumentDataRepository documentRepository
    , IDocumentDestroyer documentDestroyer)
{
    private readonly JobHeader _jobHeader = jobHeader.Value;
    private readonly ILogger<Worker> _logger = logger;
    private readonly IRecurringJobManager _recurringJobManager = recurringJobManager;
    private readonly IDocumentDataRepository _documentRepository = documentRepository;
    private readonly IDocumentDestroyer _documentDestroyer = documentDestroyer;

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
    public async Task TruncateDeadReports()
    {
        _logger.LogInformation("Truncate Dead Reports started at: {ExecuteTime}", DateTime.Now);
        var documentIdList = await _documentRepository.GetExpiredDocumentsAsync();
        foreach (var documentId in documentIdList)
        {
            _logger.LogInformation("{DocumentId} is deleting", documentId);
            var affected = await _documentRepository.DeleteDocumentByIdAsync(new GenericDocumentRequest { DocumentId = documentId });
            //TODO@burak Need to refactor. When db operations success but delete ftp file don't, what do we do?
            if (affected == 1)
            {
                var delResponse = await _documentDestroyer.DeleteAsync(new GenericDocumentRequest { DocumentId = documentId });
                if (delResponse.StatusCode != Domain.Enums.StatusCode.Success)
                {
                    _logger.LogError("Error on ftp delete operation.{StatusCode}", delResponse.StatusCode);
                }
            }
        }
    }
}

