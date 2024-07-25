using Prometheus;

namespace GamersWorld.JobHost.Monitoring;

public static class HangfireMetrics
{
    public static readonly Counter ArchiverJobSuccessCounter = Metrics.CreateCounter("archiver_job_success_total", "Total number of successful Archiver jobs");
    public static readonly Counter ArchiverJobFailureCounter = Metrics.CreateCounter("archiver_job_failure_total", "Total number of failed Archiver jobs");
    public static readonly Counter EraserJobSuccessCounter = Metrics.CreateCounter("eraser_job_success_total", "Total number of successful Eraser jobs");
    public static readonly Counter EraserJobFailureCounter = Metrics.CreateCounter("eraser_job_failure_total", "Total number of failed Eraser jobs");
}
