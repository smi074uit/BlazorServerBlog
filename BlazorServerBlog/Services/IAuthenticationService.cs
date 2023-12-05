using SharedModels.Entities.Account;

namespace BlazorServerBlog.Services;

public interface IAuthenticationService
{
	Task<string> GetUserNameFromToken();
	Task<bool> login(UserDTO user);
	Task logout();
	bool register(RegisterRequest user);
}