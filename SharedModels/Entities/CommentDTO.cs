using System.ComponentModel.DataAnnotations;

namespace SharedModels.Entities
{
    public class CommentDTO
    {
        public int EntryId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Must be between 3 and 50 characters long")]
        public string CommentBody { get; set; }
    }
}
