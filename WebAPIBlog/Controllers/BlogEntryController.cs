using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharedModels.Entities;
using System.Security.Claims;
using WebAPIBlog.Repositories;

namespace WebAPIBlog.Controllers
{
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[Route("api/[controller]")]
	[ApiController]
	public class BlogEntryController : ControllerBase
	{
		private IBlogRepository _repository;

		public BlogEntryController(IBlogRepository repository)
		{
			this._repository = repository;
		}

		// POST: api/BlogEntry/CreateEntry
		[HttpPost("CreateEntry")]
		public async Task<ActionResult<int>> CreateBlogEntry([FromBody] BlogEntryDTO entryDTO)
		{
			string userID = GetUserIdFromLoggedInUser();

			Blog blog = await _repository.GetBlogByUser(userID);

			if (blog.Locked == true)
			{
				return BadRequest("Blog is locked");
			}

			BlogEntry entryToSave = new()
			{
				BlogId = blog.BlogId,
				EntryTitle = entryDTO.EntryTitle,
				EntryBody = entryDTO.EntryBody
			};

			BlogEntry newEntry = await _repository.AddBlogEntry(entryToSave);
			return Ok(newEntry.BlogEntryId);
		}

		// POST: api/BlogEntry/CreateComment
		[HttpPost("CreateComment")]
		public async Task<ActionResult<int>> CreateComment([FromBody] CommentDTO cDTO)
		{
			string userID = GetUserIdFromLoggedInUser();

			BlogEntry entry = await _repository.GetBlogEntryById(cDTO.EntryId);
			Blog blog = await _repository.GetBlogById(entry.BlogId);

			if (blog.Locked == true)
			{
				return BadRequest("Blog is locked");
			}

			Comment newC = await _repository.AddComment(cDTO, userID);

			return Ok(newC.CommentId);
		}

		// GET: api/BlogEntry/GetEntry/{entryId}
		[HttpGet("GetEntry/{entryId:int}")]
		public async Task<ActionResult<BlogEntry>> GetEntry([FromRoute] int entryId)
		{
			BlogEntry entry = await _repository.GetBlogEntryById(entryId);

			return Ok(entry);
		}

		// GET: api/BlogEntry/GetComment/{commentId}
		[HttpGet("GetComment/{commentId:int}")]
		public async Task<ActionResult<Comment>> GetComment([FromRoute] int commentId)
		{
			Comment c = await _repository.GetCommentById(commentId);

			return Ok(c);
		}

		// PUT: api/BlogEntry/UpdateEntry
		[HttpPut("UpdateEntry")]
		public async Task<ActionResult> UpdateEntry([FromBody] BlogEntry entry)
		{
			
			BlogEntry oldEntry = await _repository.GetBlogEntryById(entry.BlogEntryId);

			if(oldEntry is null)
			{
				return BadRequest("Entry does not exist");
			}

			IdentityUser owner = await _repository.GetBlogOwner(oldEntry.BlogId);
			string userID = GetUserIdFromLoggedInUser();

			if (!(owner.Id == userID))
			{
				return BadRequest("UserID does not match");
			}

			await _repository.UpdateBlogEntry(entry);

			return Ok();
		}

		// PUT: api/BlogEntry/UpdateComment
		[HttpPut("UpdateComment")]
		public async Task<ActionResult> UpdateComment([FromBody] Comment c)
		{
			Comment oldC = await _repository.GetCommentById(c.CommentId);

			if (oldC is null)
			{
				return BadRequest("Comment does not exist");
			}

			string userID = GetUserIdFromLoggedInUser();

			if (!(oldC.OwnerId == userID))
			{
				return BadRequest("UserID does not match");
			}

			await _repository.UpdateComment(c);

			return Ok();
		}

		// DELETE: api/BlogEntry/DeleteEntry/{entryId}
		[HttpDelete("DeleteEntry/{entryId}")]
		public async Task<ActionResult> DeleteEntry([FromRoute] int entryId)
		{
			BlogEntry entry = await _repository.GetBlogEntryById(entryId);

			IdentityUser owner = await _repository.GetBlogOwner(entry.BlogId);
			string userID = GetUserIdFromLoggedInUser();

			if (!(owner.Id == userID))
			{
				return BadRequest("UserID does not match");
			}

			await _repository.DeleteBlogEntryById(entryId);

			return Ok();
		}

		// DELETE: api/BlogEntry/DeleteComment/{commentId}
		[HttpDelete("DeleteComment/{commentId}")]
		public async Task<ActionResult> DeleteComment([FromRoute] int commentId)
		{
			Comment c = await _repository.GetCommentById(commentId);

			string userID = GetUserIdFromLoggedInUser();

			if (!(c.OwnerId == userID))
			{
				return BadRequest("UserID does not match");
			}

			await _repository.DeleteCommentById(commentId);

			return Ok();
		}

		private string GetUserIdFromLoggedInUser()
		{
			ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
			return identity.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value; //"NameIdentifier").Value;
		}
	}
}
