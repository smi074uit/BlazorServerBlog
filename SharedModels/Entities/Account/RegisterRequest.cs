using System.ComponentModel.DataAnnotations;

namespace SharedModels.Entities.Account
{
    public class RegisterRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Passwords must contain at least 6 characters")]
        public string Password { get; set; }
    }
}
