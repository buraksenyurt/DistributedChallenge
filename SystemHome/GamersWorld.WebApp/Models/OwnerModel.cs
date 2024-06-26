using System.ComponentModel.DataAnnotations;

namespace GamersWorld.WebApp.Models;
public class OwnerModel
{
    [Required]
    public string? FullName { get; set; }
    [Required]
    public string? Title { get; set; }
    public string? EmployeeId { get; set; }
}