namespace GamersWorld.Domain.Entity;

public class Report
{
    public int ReportId { get; set; }

    public Guid TraceId { get; set; }

    public string? Title { get; set; }
    public string? Expression { get; set; }

    public string? EmployeeId { get; set; }

    public string? DocumentId { get; set; }

    public DateTime InsertTime { get; set; }

    public DateTime ExpireTime { get; set; }
    public bool Deleted { get; set; } = false;
    public bool Archived { get; set; } = false;
}