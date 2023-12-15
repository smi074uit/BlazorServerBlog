using Microsoft.AspNetCore.Identity;

namespace SharedModels.Entities
{
    public class Blog
    {
        public int BlogId { get; set; }
        public string BlogTitle { get; set; }
        public string? Description { get; set; }
        public bool Locked { get; set; }
        public virtual IdentityUser Owner { get; set; } = new();
        public string OwnerId { get; set; } = "";
    }
}
