using System.ComponentModel.DataAnnotations;

namespace Eval.AuditApi.Requests;
public class ExpressionCheckRequest
{
    [Required(ErrorMessage = "Expression must be filled.")]
    [StringLength(100, MinimumLength = 30, ErrorMessage = "Expression length must be between 30 and 100 characters.")]
    public string? Expression { get; set; }
    [Required(ErrorMessage = "Source must be filled")]
    public string? Source { get; set; }
}
