namespace SharedModels.Entities
{
    public class BlogViewModel
    {
        public Blog Blog { get; set; }
        public List<BlogEntry> BlogEntries { get; set; } = new();
        public List<Comment> Comments { get; set; }
    }
}
