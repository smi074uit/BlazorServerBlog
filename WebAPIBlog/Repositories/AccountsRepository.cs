using Microsoft.AspNetCore.Identity;
using SharedModels.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SharedModels.Entities.Account;
using WebAPIBlog.Data;

namespace WebAPIBlog.Repositories
{
	public class AccountsRepository : IAccountsRepository
	{
		private readonly ApplicationDbContext _db;
		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IConfiguration _conf;

		public AccountsRepository(
			SignInManager<IdentityUser> _manager,
			UserManager<IdentityUser> _userManager,
			RoleManager<IdentityRole> _roleManager,
			ApplicationDbContext _db,
			IConfiguration conf
			)
		{
			this._db = _db;
			_conf = conf;
			this._signInManager = _manager;
			this._userManager = _userManager;
			this._roleManager = _roleManager;
		}

		/// <summary>
		/// Verifies user login
		/// </summary>
		/// <see cref="User"/>
		/// <param name="user">User object to be verified</param>
		/// <returns>User object with a jwt bearer token</returns>
		public async Task<User> VerifyCredentials(UserDTO user)
		{
			if (user.Username == null || user.Password == null || user.Username.Length == 0 || user.Password.Length == 0)
			{
				return null;
			}

			var thisUser = await _userManager.FindByNameAsync(user.Username);
			if (thisUser == null)
				return (null);

			var result = await _signInManager.PasswordSignInAsync(user.Username, user.Password, false, lockoutOnFailure: true);
			if (!result.Succeeded)
			{
				return null;
			}

			var role = await _userManager.GetRolesAsync(thisUser);
			return new User() { Id = thisUser.Id, Username = user.Username, Role = role.FirstOrDefault() };
		}

		/// <summary>
		/// Generates a token for a user
		/// </summary>
		/// <param name="user">User token will be generated for</param>
		/// <returns>Jwt token string</returns>
		public string GenerateJwtToken(User user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var confKey = _conf.GetSection("TokenSettings")["SecretKey"];
			var key = Encoding.ASCII.GetBytes(confKey);
			var cIdentity = new ClaimsIdentity(new Claim[]
				{
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
					new Claim(ClaimTypes.NameIdentifier, user.Id),
					new Claim(ClaimTypes.Name, user.Username),
                    //new Claim("roles", user.Role)
                });
			if (!string.IsNullOrEmpty(user.Role))
			{
				cIdentity.AddClaim(new Claim(ClaimTypes.Role, user.Role));
			}

			//claims.AddRange(roles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = cIdentity,
				Expires = DateTime.UtcNow.AddHours(24),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			var tokenString = tokenHandler.WriteToken(token);

			return tokenString;


		}

		public Task<bool> ChangePasswd(User u, string oldP, string newP)
		{
			throw new NotImplementedException();
		}

		public Task<User> ChangeRole(User u, string newR)
		{
			throw new NotImplementedException();
		}
		public async Task LogoutUser()
		{
			await _signInManager.SignOutAsync();
			/*
            var thisUser = await _userManager.FindByNameAsync(user.Username);
            if (thisUser != null)
            {
                await _signInManager.SignOutAsync();
            }
              */
		}

		public Task<bool> DeleteUser(User u)
		{
			throw new NotImplementedException();
		}

		public async Task<User> AddUser(RegisterRequest u)
		{
			IdentityUser newUser = new()
			{
				UserName = u.Username,
			};

			IdentityResult result = await _userManager.CreateAsync(newUser, u.Password);
			if (!result.Succeeded)
			{
				return null;
			}

			if (!await _roleManager.RoleExistsAsync("User"))
			{
				await _roleManager.CreateAsync(new IdentityRole(){Name = "User"});
			}

			await _userManager.AddToRoleAsync(newUser, "User");
			var registeredUser = await _userManager.FindByNameAsync(u.Username);
			
			if (registeredUser == null)
				return (null);

			var userRole = await _userManager.GetRolesAsync(registeredUser);
			User user = new()
			{
				Id = registeredUser.Id,
				Username = registeredUser.UserName,
				Passwd = registeredUser.PasswordHash,
				Role = userRole.FirstOrDefault()
			};

			return user;

		}

		public Task<List<User>> GetAllUsers()
		{
			throw new NotImplementedException();
		}
	}
}
