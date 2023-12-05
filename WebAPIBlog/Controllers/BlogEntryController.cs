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
		public async Task<IActionResult> CreateBlogEntry([FromBody] BlogEntryDTO entryDTO)
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
			return CreatedAtAction("Get", new { id = newEntry.BlogEntryId }, newEntry);
		}

		// POST: api/BlogEntry/CreateComment
		[HttpPost("CreateComment")]
		public async Task<IActionResult> CreateComment([FromBody] CommentDTO cDTO)
		{
			string userID = GetUserIdFromLoggedInUser();

			BlogEntry entry = await _repository.GetBlogEntryById(cDTO.EntryId);
			Blog blog = await _repository.GetBlogById(entry.BlogId);

			if (blog.Locked == true)
			{
				return BadRequest("Blog is locked");
			}

			Comment newC = await _repository.AddComment(cDTO, userID);

			return CreatedAtAction("Get", new { id = newC.CommentId }, newC);
		}

		// GET: api/BlogEntry/GetEntry/{entryId}
		[HttpGet("GetEntry/{entryId:int}")]
		public async Task<BlogEntry> GetEntry([FromRoute] int entryId)
		{
			BlogEntry entry = await _repository.GetBlogEntryById(entryId);

			return entry;
		}

		// GET: api/BlogEntry/GetComment/{commentId}
		[HttpGet("GetComment/{commentId:int}")]
		public async Task<Comment> GetComment([FromRoute] int commentId)
		{
			Comment c = await _repository.GetCommentById(commentId);

			return c;
		}

		// PUT: api/BlogEntry/UpdateEntry/{entryId}
		[HttpPut("UpdateEntry/{entryId}")]
		public async Task<IActionResult> UpdateEntry([FromBody] BlogEntryDTO entryDTO, [FromRoute] int entryId)
		{
			BlogEntry entry = await _repository.GetBlogEntryById(entryId);

			IdentityUser owner = await _repository.GetBlogOwner(entry.BlogId);
			string userID = GetUserIdFromLoggedInUser();

			if (!(owner.Id == userID))
			{
				return BadRequest("UserID does not match");
			}

			if(!(entry.BlogId == entryDTO.BlogId))
			{
				return BadRequest("BlogID does not match");
			}


			entry.EntryTitle = entryDTO.EntryTitle;
			entry.EntryBody = entryDTO.EntryBody;

			await _repository.UpdateBlogEntry(entry);

			return Ok();
		}

		// PUT: api/BlogEntry/UpdateComment/{commentId}
		[HttpPut("UpdateComment/{commentId}")]
		public async Task<IActionResult> UpdateComment([FromBody] CommentDTO cDTO, [FromRoute] int commentId)
		{
			Comment c = await _repository.GetCommentById(commentId);

			string userID = GetUserIdFromLoggedInUser();

			if (!(c.OwnerId == userID))
			{
				return BadRequest("UserID does not match");
			}

			if (!(c.EntryId == cDTO.EntryId))
			{
				return BadRequest("EntryID does not match");
			}


			c.CommentBody = cDTO.CommentBody;
			await _repository.UpdateComment(c);

			return Ok();
		}

		// DELETE: api/BlogEntry/DeleteEntry/{entryId}
		[HttpDelete("DeleteEntry/{entryId}")]
		public async Task<IActionResult> DeleteEntry([FromRoute] int entryId)
		{
			BlogEntry entry = await _repository.GetBlogEntryById(entryId);

			IdentityUser owner = await _repository.GetBlogOwner(entry.BlogId);
			string userID = GetUserIdFromLoggedInUser();

			if (!(owner.Id == userID))
			{
				return BadRequest("UserID does not match");
			}

			await _repository.DeleteBlogEntryById(entryId);

			return Ok(entry);
		}

		// DELETE: api/BlogEntry/DeleteComment/{commentId}
		[HttpDelete("DeleteComment/{commentId}")]
		public async Task<IActionResult> DeleteComment([FromRoute] int commentId)
		{
			Comment c = await _repository.GetCommentById(commentId);

			string userID = GetUserIdFromLoggedInUser();

			if (!(c.OwnerId == userID))
			{
				return BadRequest("UserID does not match");
			}

			await _repository.DeleteCommentById(commentId);

			return Ok(c);
		}

		private string GetUserIdFromLoggedInUser()
		{
			ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
			return identity.Claims.FirstOrDefault(x => x.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value; //"NameIdentifier").Value;
		}
	}
}
