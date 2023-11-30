using Microsoft.AspNetCore.Identity;
using SharedModels.Entities;
using SharedModels.Entities.Account;

namespace WebAPIBlog.Repositories
{
	public interface IAccountsRepository
	{

		Task<User> VerifyCredentials(UserDTO user);
		string GenerateJwtToken(User user);
		Task<bool> ChangePasswd(User u, string oldP, string newP);
		Task<User> ChangeRole(User u, string newR);
		Task<bool> DeleteUser(User u);
		Task LogoutUser();
		Task<User> AddUser(RegisterRequest u);
		Task<List<User>> GetAllUsers();
	}
}
