using Microsoft.Extensions.Hosting;
using Prometheus;

namespace GamersWorld.JobHost.Monitoring;

public class MetricsServer
    : IHostedService
{
    private readonly KestrelMetricServer _metricServer = new(port: 1903);
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _metricServer.Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _metricServer.StopAsync();
    }
}
