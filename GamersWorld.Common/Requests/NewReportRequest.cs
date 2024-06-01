namespace GamersWorld.Common.Messages.Requests;

using System.ComponentModel.DataAnnotations;

public class NewReportRequest
{
    [Required(ErrorMessage = "Report must has a title.")]
    [StringLength(30, MinimumLength = 20, ErrorMessage = "Title length must be between 20 and 30 characters.")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Expression must be filled.")]
    [StringLength(100, MinimumLength = 30, ErrorMessage = "Expression length must be between 30 and 100 characters.")]
    public string Expression { get; set; }
}