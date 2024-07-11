namespace GamersWorld.Domain.Dtos;

public record ReportNotification
{
    public string? DocumentId { get; set; }
    public string? Content { get; set; }
    public bool IsSuccess { get; set; } = true;
}