using System.ComponentModel.DataAnnotations;

namespace GamersWorld.Domain.Requests;

public class GetReportsByEmployeeRequest
{
    [Required]
    public string? EmployeeId { get; set; }
}