namespace Kahin.Common.Requests;

using Kahin.Common.Entities;
using Kahin.Common.Enums;

public class RedisPayload
{
    public string? TraceId { get; set; }
    public ReferenceDocumentId DocumentId { get; set; }
    public string? Expression { get; set; }
    public EventType EventType { get; set; }

    override public string ToString()
    {
        return string.Format($"TraceId: {TraceId},EventType: {EventType}, ReferenceDocumentId: {DocumentId}");
    }

    public static RedisPayload Default() => new()
    {
        TraceId = null,
        DocumentId = new ReferenceDocumentId(),
        Expression = null,
        EventType = EventType.NotActive
    };
}