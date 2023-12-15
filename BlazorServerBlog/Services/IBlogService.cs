using SharedModels.Entities;

namespace BlazorServerBlog.Services
{
    public interface IBlogService
    {
        Task<int> CreateBlog(BlogDTO blog);
        Task<bool> DoesUserHaveBlog();
        Task<BlogViewModel> GetBlogEntries(int blogId);
        Task<int> GetBlogIdByUsername(string username);
        Task<IEnumerable<Blog>> GetBlogs();
        bool ToggleBlogLock();
        Task<Blog> UpdateBlog(BlogDTO blogDTO);
    }
}