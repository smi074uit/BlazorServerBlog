using SharedModels.Entities;

namespace BlazorServerBlog.Services
{
	public interface IBlogService
	{
		Task<int> CreateBlog(BlogDTO blog);
		Task<BlogViewModel> GetBlogEntries(int blogId);
		Task<IEnumerable<Blog>> GetBlogs();
		bool ToggleBlogLock();
		Task<Blog> UpdateBlog(BlogDTO blogDTO);
	}
}