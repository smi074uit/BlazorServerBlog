using System.ComponentModel.DataAnnotations;

namespace SharedModels.Entities.Account
{
    public class RegisterRequest
    {
        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Username must be between 3-20 characters")]
        public string Username { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Passwords must contain at least 6 characters")]
        public string Password { get; set; }
    }
}
