using System.ComponentModel.DataAnnotations;

namespace GamersWorld.WebApp.Models;

public class ReportRequestModel
{
    [Required]
    public OwnerModel Owner { get; set; }
    [Required]
    public string? ReportTitle { get; set; }
    [Required]
    public string? Expression { get; set; }
    public DateTime RequestTime { get; set; } = DateTime.Now;
}