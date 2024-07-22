using GamersWorld.Application.Contracts.Data;
using GamersWorld.Application.Contracts.Document;
using GamersWorld.Domain.Constants;
using GamersWorld.Domain.Requests;
using GamersWorld.JobHost.Model;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GamersWorld.JobHost;

//TODO@burak Need to refactor because of dependent objects count increases for every new job business

internal class Worker(
    IOptions<JobHeader> jobHeader
    , ILogger<Worker> logger
    , IRecurringJobManager recurringJobManager
    , IReportDataRepository reportDataRepository
    , IReportDocumentDataRepository reportDocumentDataRepository
    , IDocumentDestroyer documentDestroyer
    , IServiceProvider serviceProvider)
{
    private readonly JobHeader _jobHeader = jobHeader.Value;
    private readonly ILogger<Worker> _logger = logger;
    private readonly IRecurringJobManager _recurringJobManager = recurringJobManager;
    private readonly IReportDataRepository _reportDataRepository = reportDataRepository;
    private readonly IReportDocumentDataRepository _reportDocumentDataRepository = reportDocumentDataRepository;
    private readonly IDocumentDestroyer _documentDestroyer = documentDestroyer;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task ExecuteJobs()
    {
        foreach (var job in _jobHeader.Jobs.Where(j => j.Active))
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
        var documentIdList = await _reportDataRepository.GetExpiredReportsAsync(interval: TimeSpan.FromHours(1));
        foreach (var documentId in documentIdList)
        {
            var request = new GenericDocumentRequest { DocumentId = documentId };
            _logger.LogInformation("{DocumentId} is deleting", documentId);
            var affected = await _reportDocumentDataRepository.DeleteDocumentAsync(documentId);
            if (affected != 1)
            {
                _logger.LogWarning("Error on 'delete document row' for {DocumentId}", documentId);
            }
            var report = await _reportDataRepository.ReadReportAsync(documentId);
            report.Deleted = true;
            var markResponse = await _reportDataRepository.UpdateReportAsync(report);
            if (markResponse != 1)
            {
                _logger.LogWarning("Error on 'marked as deleted' for {DocumentId}", documentId);
            }

            var delResponse = await _documentDestroyer.DeleteAsync(request);
            if (delResponse.StatusCode != Domain.Enums.StatusCode.Success)
            {
                _logger.LogError("Error on ftp delete operation.{StatusCode}", delResponse.StatusCode);
            }
        }
    }

    public async Task ArchiveExpiredReports()
    {
        var documentWriter = _serviceProvider.GetRequiredKeyedService<IDocumentWriter>(Names.FtpWriteService);
        _logger.LogInformation("Archive the expired reports to ftp process started at: {ExecuteTime}", DateTime.Now);
        var documentIdList = await _reportDataRepository.GetExpiredReportsAsync();
        foreach (var documentId in documentIdList)
        {
            var report = await _reportDataRepository.ReadReportAsync(documentId);
            report.Archived = true;
            var updatedCount = await _reportDataRepository.UpdateReportAsync(report);
            if (updatedCount == 1)
            {
                var doc = await _reportDocumentDataRepository.ReadDocumentAsync(documentId);
                if (doc == null)
                {
                    _logger.LogWarning("{DocumentId} content not found", documentId);
                    continue;
                }
                else
                {
                    var uploadResponse = await documentWriter.SaveAsync(
                        new ReportSaveRequest
                        {
                            DocumentId = documentId,
                            Content = doc.Content
                        });

                    if (uploadResponse.StatusCode != Domain.Enums.StatusCode.DocumentUploaded)
                    {
                        _logger.LogError("Error on ftp upload operation.{StatusCode}", uploadResponse.StatusCode);
                    }
                }
            }
        }
        //TODO@buraksenyurt Push an event for update all clients dataset (Push to Rabbit and than nofity with signalR)
    }
}