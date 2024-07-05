namespace Resistance;

public class ResistanceOptions
{
    public bool NetworkFailureIsActive { get; set; } = false;
    public NetworkFailureProbability NetworkFailureProbability { get; set; } = NetworkFailureProbability.Percent10;
    public bool LatencyIsActive { get; set; } = false;
    public LatencyPeriod LatencyPeriod { get; set; } = new LatencyPeriod();
    public bool ResourceRaceIsActive { get; set; } = false;
    public ushort ResourceRaceUpperLimit { get; set; } = 2;
    public bool OutageIsActive { get; set; } = false;
    public OutagePeriod OutagePeriod { get; set; } = new OutagePeriod();
}