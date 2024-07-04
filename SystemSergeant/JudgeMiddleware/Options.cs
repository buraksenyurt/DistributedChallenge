namespace JudgeMiddleware;

public class Options
{
    public TimeSpan DurationThreshold { get; set; } = TimeSpan.FromSeconds(3);
    public bool DeactivatePerformanceBehavior { get; set; } = false;
    public bool DeactivateInputOutputBehavior { get; set; } = false;
    public List<string> ExcludedPaths { get; set; } = [];
}