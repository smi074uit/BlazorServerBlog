﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            if(!(registerRequest == null)) {
                if(registerRequest.Username == null || 3 < registerRequest.Username.Length || registerRequest.Username.Length < 50 )
                {
                    return BadRequest("username not valid");
                }
                if(registerRequest.Password == null || 6 < registerRequest.Password.Length || registerRequest.Password.Length < 50)
                {
                    return BadRequest("password not valid");
                }

			}

            User res = await _repo.AddUser(registerRequest);

            if (res == null)
            {
                return BadRequest();
            }


            return Ok();
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("getAllUsernames")]
        public async Task<ActionResult<IEnumerable<string>>> GetAllUsernames()
        {
            List<string> res = await _repo.GetAllUsernames();

            return Ok(res);
        }
    }
}
