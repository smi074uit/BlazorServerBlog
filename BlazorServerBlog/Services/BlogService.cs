using SharedModels.Entities;

namespace BlazorServerBlog.Services
{
    public class BlogService : IBlogService
    {
        private readonly IApiHelper api;

        public BlogService(IApiHelper api)
        {
            this.api = api;
        }
        public async Task<IEnumerable<Blog>> GetBlogs()
        {
            var response = api.GetData("Blog/GetAll");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            IEnumerable<Blog> result = await response.Content.ReadFromJsonAsync<IEnumerable<Blog>>();

            return result;
        }

        public async Task<BlogViewModel> GetBlogEntries(int blogId)
        {
            var response = api.GetData("Blog/GetBlogViewModel/" + blogId.ToString());

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            BlogViewModel result = await response.Content.ReadFromJsonAsync<BlogViewModel>();

            return result;
        }
		
		public async Task<int> CreateBlog(BlogDTO blog)
        {
            var response = api.PostData<BlogDTO>(blog, "Blog/CreateBlog");

            if (!response.IsSuccessStatusCode)
            {
                return -1;
            }

            string result = await response.Content.ReadAsStringAsync();

            int intResult = Int32.Parse(result);

            return intResult;
        }

        public async Task<Blog> UpdateBlog(BlogDTO blogDTO)
        {
            var response = api.PutData<BlogDTO>(blogDTO, "Blog/UpdateBlog");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            Blog result = await response.Content.ReadFromJsonAsync<Blog>();

            return result;
        }

        public bool ToggleBlogLock()
        {

            var response = api.PostData<dynamic>(new { }, "Blog/ToggleLock");

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> DoesUserHaveBlog()
        {
            var response = api.GetData("Blog/DoesUserHaveBlog");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error fetching user blog status");
            }

            bool result = await response.Content.ReadFromJsonAsync<bool>();

            return result;
        }

        public async Task<int> GetBlogIdByUsername(string username)
        {
            var response = api.GetData("Blog/GetBlogIdByUsername/" + username);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error fetching user blog");
            }

            int result = await response.Content.ReadFromJsonAsync<int>();

            return result;
        }
    }
}
