using GamersWorld.SDK;

namespace GamersWorld.Events;

public class ReportRequestedEvent : IEvent
{
    public Guid TraceId { get; set; }
    public string? EmployeeId { get; set; }
    public string Title { get; set; } = "Default";
    public string Expression { get; set; } = "Select * From TopSalariesView Order By Amount";
    public DateTime Time { get; set; }
}
