using GamersWorld.SDK;

namespace GamersWorld.Events;

public class ReportProcessCompletedEvent : IEvent
{
    public Guid TraceId { get; set; }
    public DateTime Time { get; set; }
    public Guid CreatedReportId { get; set; }
}
