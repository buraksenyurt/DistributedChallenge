namespace Resistance;

public class LatencyPeriod
{
    public TimeSpan MinDelayMs { get; set; } = TimeSpan.FromMilliseconds(100);
    public TimeSpan MaxDelayMs { get; set; } = TimeSpan.FromMilliseconds(500);
}