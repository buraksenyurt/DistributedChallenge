namespace GamersWorld.WebApp.Models;

public record ReportViewModel
{
    public string? EmployeeId { get; set; }
    public IEnumerable<ReportModel>? Reports { get; set; }
}
