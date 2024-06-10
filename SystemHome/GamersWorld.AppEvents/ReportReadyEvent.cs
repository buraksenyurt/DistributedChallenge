using GamersWorld.SDK;

namespace GamersWorld.AppEvents;

public class ReportReadyEvent : IEvent
{
    public Guid TraceId { get; set; }
    public DateTime Time { get; set; }
    public string? CreatedReportId { get; set; }
}