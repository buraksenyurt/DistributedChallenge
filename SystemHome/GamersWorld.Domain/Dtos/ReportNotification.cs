using GamersWorld.Domain.Enums;

namespace GamersWorld.Domain.Dtos;

public record ReportNotification
{
    public string? DocumentId { get; set; }
    public string? Content { get; set; }
    public bool IsSuccess { get; set; } = true;
    public string Topic { get; set; } = NotificationTopic.None.ToString();
}