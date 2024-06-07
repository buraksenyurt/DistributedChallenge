using System.ComponentModel.DataAnnotations;

namespace GamersWorld.WebApp.Models;
public struct OwnerModel
{
    [Required]
    public string? FullName { get; set; }
    [Required]
    public string? Title { get; set; }
}