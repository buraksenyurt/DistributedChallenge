namespace Resistance;

public class Options
{
    public bool NetworkFailureIsActive { get; set; } = false;
    public NetworkFailureProbability NetworkFailureProbability { get; set; } = NetworkFailureProbability.Percent10;
    public bool LatencyIsActive { get; set; } = false;
}