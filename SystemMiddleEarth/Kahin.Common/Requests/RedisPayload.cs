namespace Kahin.Common.Requests;

using Kahin.Common.Entities;

public class RedisPayload
{
    public ReferenceDocumentId DocumentId { get; set; }
    public string? Expression { get; set; }
}