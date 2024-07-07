namespace GamersWorld.Domain.Dtos;

public record ReportNotification
{
    public string? DocumentId { get; set; }
    public string? Title { get; set; }
}