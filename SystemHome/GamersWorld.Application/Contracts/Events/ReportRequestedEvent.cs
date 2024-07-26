using GamersWorld.Domain.Enums;

namespace GamersWorld.Application.Contracts.Events;

public class ReportRequestedEvent : IEvent
{
    public BaseEventData EventData { get; set; } = new BaseEventData();
    public string? EmployeeId { get; set; }
    public string Title { get; set; } = "Default";
    public string Expression { get; set; } = "Select * From TopSalariesView Order By Amount";
    public Lifetime Lifetime { get; set; }
}
