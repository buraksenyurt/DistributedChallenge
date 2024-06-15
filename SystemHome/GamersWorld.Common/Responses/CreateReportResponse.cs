using System.Text.Json.Serialization;
using GamersWorld.Common.Enums;

namespace GamersWorld.Common.Responses
{
    public class CreateReportResponse
    {
        [JsonPropertyName("status")]
        public StatusCode Status { get; set; }
        [JsonPropertyName("documentId")]
        public string? DocumentId { get; set; }
        [JsonPropertyName("explanation")]
        public string? Explanation { get; set; }
    }    
}
