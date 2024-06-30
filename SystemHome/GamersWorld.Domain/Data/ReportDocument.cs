using System.Text.Json.Serialization;

namespace GamersWorld.Domain.Data;

public class ReportDocument
{
    public int Id { get; set; }
    public Guid TraceId { get; set; }
    public string? EmployeeId { get; set; }
    public string? DocumentId { get; set; }
    [JsonIgnore]
    public byte[]? Content { get; set; }
    public DateTime InsertTime { get; set; }
}