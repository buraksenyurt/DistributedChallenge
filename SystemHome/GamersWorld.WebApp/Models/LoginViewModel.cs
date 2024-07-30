using System.ComponentModel.DataAnnotations;

namespace GamersWorld.WebApp.Models
{
    public class LoginViewModel
    {
        [Required]
        public string RegistrationId { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
