using System.ComponentModel.DataAnnotations;

namespace SharedModels.Entities
{
    public class BlogEntryDTO
    {
        public int BlogEntryId { get; set; }
        public int BlogId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Must be between 3 and 50 characters long")]
        public string EntryTitle { get; set; }

        [Required]
        public string EntryBody { get; set; }

        public string? TagsString { get; set; } = "";
    }
}
