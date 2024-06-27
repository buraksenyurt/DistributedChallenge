using System.ComponentModel.DataAnnotations;

namespace GamersWorld.Common.Requests;

public class GetReportsByEmployeeRequest
{
    [Required]
    public string? EmployeeId { get; set; }
}