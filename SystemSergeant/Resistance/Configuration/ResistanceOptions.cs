using Resistance.Inconsistency;
using Resistance.Latency;
using Resistance.NetworkFailure;
using Resistance.Outage;

namespace Resistance.Configuration;

public class ResistanceOptions
{
    public NetworkFailureProbability NetworkFailureProbability { get; set; } = NetworkFailureProbability.Percent10;
    public LatencyPeriod LatencyPeriod { get; set; } = new LatencyPeriod();
    public ushort ResourceRaceUpperLimit { get; set; } = 2;
    public OutagePeriod OutagePeriod { get; set; } = new OutagePeriod();
    public DataInconsistencyProbability DataInconsistencyProbability { get; set; } = DataInconsistencyProbability.Percent20;
}