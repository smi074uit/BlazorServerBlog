using SharedModels.Entities;

namespace BlazorServerBlog.Services
{
    public interface IBlogEntryService
    {
        Task<int> CreateBlogEntry(BlogEntryDTO entryDTO);
        Task<int> CreateComment(CommentDTO cDTO);
        Task<bool> DeleteComment(int commentId);
        Task<bool> DeleteEntry(int entryId);
        Task<Comment> GetComment(int commentId);
        Task<BlogEntry> GetEntry(int entryId);
        Task<bool> UpdateComment(Comment c);
        Task<bool> UpdateEntry(BlogEntryDTO entry);
    }
}