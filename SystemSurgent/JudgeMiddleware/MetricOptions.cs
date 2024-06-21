namespace JudgeMiddleware;

public class MetricOptions
{
    public TimeSpan DurationThreshold { get; set; } = TimeSpan.FromSeconds(3);
}