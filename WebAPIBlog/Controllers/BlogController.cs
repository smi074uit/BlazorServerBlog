using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedModels.Entities;
using System.Security.Claims;
using System.Security.Principal;
using WebAPIBlog.Repositories;

namespace WebAPIBlog.Controllers
{
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[Route("api/[controller]")]
	[ApiController]
	public class BlogController : ControllerBase
	{
		private IBlogRepository _repository;

		public BlogController(IBlogRepository repository)
		{
			this._repository = repository;
		}

		// GET: api/Blog/GetAll
		[AllowAnonymous]
		[HttpGet("GetAll")]
		public async Task<ActionResult<IEnumerable<Blog>>> GetBlogs()
		{
			IEnumerable<Blog> res = await _repository.GetAll();
			return Ok(res);
		}

		// GET: api/Blog/GetBlogViewModel/{blogId}
		[AllowAnonymous]
		[HttpGet("GetBlogViewModel/{blogId:int}")]
		public async Task<ActionResult<BlogViewModel>> GetBlogEntries(int blogId)
		{
			Blog blog = await _repository.GetBlogById(blogId);
			List<BlogEntry> entries = await _repository.GetBlogEntriesByBlogId(blogId);
			List<Comment> comments = await _repository.GetComments(entries);

			BlogViewModel viewData = new()
			{
				Blog = blog,
				BlogEntries = entries,
				Comments = comments
			};

			return Ok(viewData);
		}

		// POST: api/Blog/CreateBlog
		[HttpPost("CreateBlog")]
		public async Task<ActionResult<int>> CreateBlog([FromBody] BlogDTO blog)
		{
			string userID = GetUserIdFromLoggedInUser();

			await _repository.AddBlog(blog, userID);

			Blog newBlog = await _repository.GetBlogByUser(userID);

			return Ok(newBlog.BlogId);
		}

		// PUT: api/Blog/UpdateBlog
		[HttpPut("UpdateBlog")]
		public async Task<ActionResult<Blog>> UpdateBlog([FromBody] BlogDTO blogDTO)
		{
			string userID = GetUserIdFromLoggedInUser();

			Blog blog = await _repository.GetBlogByUser(userID);

			blog.BlogTitle = blogDTO.BlogTitle;
			blog.Description = blogDTO.Description;

			await _repository.UpdateBlog(blog);

			return Ok(blog);
		}

		// POST: api/Blog/ToggleLock
		[HttpPost("ToggleLock")]
		public async Task<ActionResult> ToggleBlogLock()
		{

			string userID = GetUserIdFromLoggedInUser();

			await _repository.toggleBlogLock(userID);

			return Ok();
		}

		private string GetUserIdFromLoggedInUser()
		{
			ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
			return identity.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value; //"NameIdentifier").Value;
		}
	
	}
}

