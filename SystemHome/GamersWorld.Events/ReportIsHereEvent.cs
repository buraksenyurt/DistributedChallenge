using GamersWorld.SDK;

namespace GamersWorld.Events;

public class ReportIsHereEvent : IEvent
{
    public Guid TraceId { get; set; }
    public DateTime Time { get; set; }
    public string? CreatedReportId { get; set; }
}
