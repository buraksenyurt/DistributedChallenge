namespace Loki.Requests.Kahin;

public class CreateReportRequest
{
    public string? TraceId { get; set; }

    public string? EmployeeId { get; set; }

    public string? Title { get; set; }

    public string? Expression { get; set; }
}