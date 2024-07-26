using GamersWorld.Application.Contracts.Data;
using GamersWorld.Application.Contracts.Document;
using GamersWorld.Application.Tasking;
using GamersWorld.Domain.Requests;
using GamersWorld.JobHost.Monitoring;
using Hangfire;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace GamersWorld.JobHost.Business
{
    public class ReportEraser(ILogger<ReportEraser> logger, IReportDataRepository reportDataRepository, IReportDocumentDataRepository reportDocumentDataRepository, IDocumentDestroyer documentDestroyer)
        : IJobAction
    {
        [JobDisplayName("Eraser Job")]
        public async Task Execute()
        {
            logger.LogInformation("Truncate the Archived Reports started at: {ExecuteTime}", DateTime.Now);
            var timer = HangfireMetrics.EraserJobDuration.NewTimer();
            try
            {
                var documentIdList = await reportDataRepository.GetExpiredReportsAsync(interval: TimeSpan.FromHours(1));
                foreach (var documentId in documentIdList)
                {
                    var request = new GenericDocumentRequest { DocumentId = documentId };
                    logger.LogInformation("{DocumentId} is deleting", documentId);
                    var affected = await reportDocumentDataRepository.DeleteDocumentAsync(documentId);
                    if (affected != 1)
                    {
                        logger.LogWarning("Error on 'delete document row' for {DocumentId}", documentId);
                    }
                    var report = await reportDataRepository.ReadReportAsync(documentId);
                    report.Deleted = true;
                    var markResponse = await reportDataRepository.UpdateReportAsync(report);
                    if (markResponse != 1)
                    {
                        logger.LogWarning("Error on 'marked as deleted' for {DocumentId}", documentId);
                    }

                    var delResponse = await documentDestroyer.DeleteAsync(request);
                    if (delResponse.Status != Domain.Enums.Status.Success)
                    {
                        logger.LogError("Error on ftp delete operation.{StatusCode}", delResponse.Status);
                    }
                }
                HangfireMetrics.EraserJobSuccessCounter.Inc();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception on report ereasing job");
                HangfireMetrics.EraserJobFailureCounter.Inc();
            }
            finally
            {
                timer.ObserveDuration();
            }
            logger.LogInformation("Truncate the Archived Reports has been completed at: {ExecuteTime}", DateTime.Now);
        }
    }
}
