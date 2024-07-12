using System.ComponentModel.DataAnnotations;

namespace GamersWorld.Domain.Requests
{
    public class DeleteDocumentByIdRequest
    {
        [Required]
        public string? DocumentId { get; set; } 
    }
}
