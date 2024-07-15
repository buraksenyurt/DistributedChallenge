namespace GamersWorld.Application.Contracts.Events;

public class ArchiveReportEvent : IEvent
{
    public Guid TraceId { get; set; }
    public string? DocumentId { get; set; }
    public string? Title { get; set; }
    public DateTime Time { get; set; }
    public string? ClientId { get; set; }
}
