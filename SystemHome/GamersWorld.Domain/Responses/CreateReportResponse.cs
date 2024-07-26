using System.Text.Json.Serialization;
using GamersWorld.Domain.Enums;

namespace GamersWorld.Domain.Responses
{
    public class CreateReportResponse
    {
        [JsonPropertyName("status")]
        public Status Status { get; set; }
        [JsonPropertyName("documentId")]
        public string? DocumentId { get; set; }
        [JsonPropertyName("explanation")]
        public string? Explanation { get; set; }
    }    
}
