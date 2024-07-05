namespace Resistance.Outage;

public class OutagePeriod
{
    public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(10);
    public TimeSpan Frequency { get; set; } = TimeSpan.FromMinutes(2);
}
