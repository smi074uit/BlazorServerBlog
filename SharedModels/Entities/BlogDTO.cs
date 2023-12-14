using System.ComponentModel.DataAnnotations;

namespace SharedModels.Entities
{
    public class BlogDTO
    {
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Must be between 3 and 50 characters long")]
        public string BlogTitle { get; set; }
        [StringLength(100, ErrorMessage = "Must be less than 100 characters")]
        public string? Description { get; set; }
    }
}
