using System.ComponentModel.DataAnnotations;

namespace GamersWorld.Domain.Requests
{
    public class GetReportDocumentByIdRequest
    {
        [Required]
        public string? DocumentId { get; set; }
    }
}
