namespace Kahin.Common.Requests;

using Kahin.Common.Entities;
using Kahin.Common.Enums;

public class RedisPayload
{
    public string? TraceId { get; set; }
    public string? EmployeeId { get; set; }
    public string? ReportTitle { get; set; }
    public ReferenceDocumentId DocumentId { get; set; }
    public string? Expression { get; set; }
    public EventType EventType { get; set; }
    public TimeSpan ReportExpireTime { get; set; }

    public override string ToString() 
        => string.Format($"TraceId: {TraceId},EventType: {EventType}, ReferenceDocumentId: {DocumentId}");

    public static RedisPayload Default() => new()
    {
        TraceId = null,
        DocumentId = new ReferenceDocumentId(),
        Expression = null,
        EventType = EventType.NotActive
    };
}