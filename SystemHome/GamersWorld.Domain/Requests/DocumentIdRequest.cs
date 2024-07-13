using System.ComponentModel.DataAnnotations;

namespace GamersWorld.Domain.Requests
{
    public class DocumentIdRequest
    {
        [Required]
        public string? DocumentId { get; set; } 
    }
}
