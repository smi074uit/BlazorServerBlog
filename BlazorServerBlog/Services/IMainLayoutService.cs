using SharedModels.Entities;

namespace BlazorServerBlog.Services
{
    public interface IMainLayoutService
    {
        Task<IEnumerable<Tag>> GetTags();
        Task<IEnumerable<string>> GetUsernames();
    }
}