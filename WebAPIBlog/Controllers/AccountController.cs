using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharedModels.Entities;
using SharedModels.Entities.Account;
using WebAPIBlog.Data;
using WebAPIBlog.Repositories;

namespace WebAPIBlog.Controllers
{
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly ApplicationDbContext _appDbContext;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IAccountsRepository _repo;

		public AccountController(UserManager<IdentityUser> userManager, ApplicationDbContext appDbContext,
			IAccountsRepository repo)
		{
			_userManager = userManager;
			_appDbContext = appDbContext;
			_repo = repo;
		}

		[AllowAnonymous]
		[HttpPost("verifyLogin")]
		public async Task<ActionResult<User>> VerifyLogin(UserDTO user)
		{
			//if (!ModelState.IsValid)
			//{
			//	return BadRequest(ModelState);
			//}

			User res = await _repo.VerifyCredentials(user);

			if (res == null)
			{
				return BadRequest("Brukernavn/Passord er feil");
			}

			res.Token = _repo.GenerateJwtToken(res);

			return Ok(res);
		}

		[HttpPost]
		[Route("logout")]
		public async Task<IActionResult> Logout(string returnUrl = null)
		{
			await _repo.LogoutUser();
			if (returnUrl != null)
			{
				return LocalRedirect(returnUrl);
			}

			return Ok();
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("registerNewUser")]
		public async Task<IActionResult> RegisterNewUser(RegisterRequest registerRequest)
		{
			// TODO Validate

			User res = await _repo.AddUser(registerRequest);


			return Ok();
		}
	}
}
