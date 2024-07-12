namespace Kahin.Common.Requests;

using System.ComponentModel.DataAnnotations;

public class CreateReportRequest
{
    [Required]
    public string? TraceId { get; set; }

    [Required]
    public string? EmployeeId { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 20, ErrorMessage = "Title length must be between 20 and 50 characters.")]
    public string? Title { get; set; }

    [Required(ErrorMessage = "Expression must be filled.")]
    [StringLength(200, MinimumLength = 50, ErrorMessage = "Expression length must be between 50 and 200 characters.")]
    public string? Expression { get; set; }
}