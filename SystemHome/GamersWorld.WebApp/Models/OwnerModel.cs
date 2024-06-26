using System.ComponentModel.DataAnnotations;

namespace GamersWorld.WebApp.Models;
public class OwnerModel
{
    [Required]
    public string FullName { get; set; } = "Burak Selim Şenurt";
    [Required]
    public string Title { get; set; } = "Last Sales Report";
    public string EmployeeId { get; set; } = "12345";
}