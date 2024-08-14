namespace Resistance.Configuration;

public class ResistanceFlags
{
    public bool NetworkFailureIsActive { get; set; } = false;
    public bool LatencyIsActive { get; set; } = false;
    public bool ResourceRaceIsActive { get; set; } = false;
    public bool OutageIsActive { get; set; } = false;
    public bool DataInconsistencyIsActive { get; set; } = false;
}