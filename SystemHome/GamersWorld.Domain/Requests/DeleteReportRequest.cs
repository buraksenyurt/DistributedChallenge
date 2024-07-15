namespace GamersWorld.Domain.Requests;

using System.ComponentModel.DataAnnotations;

public class DeleteReportRequest
{
    [Required(ErrorMessage = "Document Id required")]
    public string? DocumentId { get; set; }

    [Required(ErrorMessage = "Report must has a title")]
    public string? Title { get; set; }
    [Required(ErrorMessage = "Employee Id required")]
    public string? EmployeeId { get; set; }
}