namespace GamersWorld.Domain.Requests;

using GamersWorld.Domain.Enums;
using System.ComponentModel.DataAnnotations;

public class NewReportRequest
{
    [Required(ErrorMessage = "Report must has a title.")]
    [StringLength(50, MinimumLength = 20, ErrorMessage = "Title length must be between 20 and 50 characters.")]
    public string? Title { get; set; }

    [Required(ErrorMessage = "Report must has Owner's Employee Id")]
    public string? EmployeeId { get; set; }

    [Required(ErrorMessage = "Expression must be filled.")]
    [StringLength(200, MinimumLength = 50, ErrorMessage = "Expression length must be between 50 and 200 characters.")]
    public string? Expression { get; set; }
    public Lifetime Lifetime { get; set; }
}