using SharedModels.Entities;

namespace BlazorServerBlog.Services
{
    public class MainLayoutService : IMainLayoutService
    {
        private readonly IApiHelper api;

        public MainLayoutService(IApiHelper api)
        {
            this.api = api;
        }

        public async Task<IEnumerable<Tag>> GetTags()
        {
            var response = api.GetData("Blog/GetAllTags");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            IEnumerable<Tag> result = await response.Content.ReadFromJsonAsync<IEnumerable<Tag>>();

            return result;
        }

        public async Task<IEnumerable<string>> GetUsernames()
        {
            var response = api.GetData("Account/getAllUsernames");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            IEnumerable<string> result = await response.Content.ReadFromJsonAsync<IEnumerable<string>>();

            return result;
        }
    }
}
