namespace GamersWorld.JobHost.Model;

internal class JobInformation
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string CronExpression { get; set; }
    public string ActionType { get; set; }
    public bool Active { get; set; }
}
