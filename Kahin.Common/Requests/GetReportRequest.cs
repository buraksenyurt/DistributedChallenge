using System.ComponentModel.DataAnnotations;

namespace Kahin.Common.Requests;

public class GetReportRequest
{
    [Required]
    public string? DocumentId { get; set; }
}