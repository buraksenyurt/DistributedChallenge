using System.ComponentModel.DataAnnotations;

namespace Eval.AuditLib.Model;
public class ExpressionCheckRequest
{
    [Required(ErrorMessage = "Expression must be filled.")]
    [StringLength(200, MinimumLength = 50, ErrorMessage = "Expression length must be between 50 and 200 characters.")]
    public string? Expression { get; set; }
    [Required(ErrorMessage = "Source must be filled")]
    public string? Source { get; set; }
}
