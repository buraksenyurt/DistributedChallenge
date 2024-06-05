using System.Text.Json.Serialization;
using Kahin.Common.Entities;
using Kahin.Common.Enums;

namespace Kahin.Common.Responses;

public class CreateReportResponse
{
    public StatusCode Status { get; set; }
    [JsonIgnore]
    public ReferenceDocumentId ReferenceDocumentId { get; set; }
    public string? DocumentId { get; set; }
    public string Explanation { get; set; }
}