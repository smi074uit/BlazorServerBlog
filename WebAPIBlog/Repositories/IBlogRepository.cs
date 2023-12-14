using Microsoft.AspNetCore.Identity;
using SharedModels.Entities;

namespace WebAPIBlog.Repositories
{
    public interface IBlogRepository
    {
        Task<Blog> AddBlog(BlogDTO blogDTO, string ownerID);
        Task<BlogEntry> AddBlogEntry(BlogEntry blogEntry);
        Task<Comment> AddComment(CommentDTO cDTO, string userID);
        Task DeleteBlogEntryById(int id);
        Task DeleteCommentById(int id);
        Task<IEnumerable<Blog>> GetAll();
        Task<Blog> GetBlogById(int blogId);
        Task<Blog> GetBlogByUser(string userID);
        Task<List<BlogEntry>> GetBlogEntriesByBlogId(int id);
        Task<BlogEntry> GetBlogEntryById(int id);
        Task<IdentityUser> GetBlogOwner(int blogId);
        Task<Comment> GetCommentById(int id);
        Task<List<Comment>> GetComments(List<BlogEntry> entries);
        Task<bool> isBlogLocked(int blogId);
        Task<bool> toggleBlogLock(string userID);
        Task UpdateBlog(Blog blog);
        Task<BlogEntry> UpdateBlogEntry(BlogEntry entry);
        Task UpdateComment(Comment comment);
        Task AddTagIfNewTag(string tag);
        Task AddTagsToEntry(BlogEntry entry, List<string> tags);
        Task<IEnumerable<Tag>> GetAllTags();
    }
}