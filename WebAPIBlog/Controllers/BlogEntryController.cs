using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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

            ProcessTags(entryDTO.TagsString, newEntry);

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
            BlogEntry dbentry = await _repository.GetBlogEntryById(entryId);

            // Workaround for passing recursive data into json
            BlogEntry entry = CopyBlogEntry(dbentry);

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
        public async Task<ActionResult> UpdateEntry([FromBody] BlogEntryDTO entry)
        {

            BlogEntry oldEntry = await _repository.GetBlogEntryById(entry.BlogEntryId);

            if (oldEntry is null)
            {
                return BadRequest("Entry does not exist");
            }

            IdentityUser owner = await _repository.GetBlogOwner(oldEntry.BlogId);
            string userID = GetUserIdFromLoggedInUser();

            if (!(owner.Id == userID))
            {
                return BadRequest("UserID does not match");
            }

            oldEntry.EntryTitle = entry.EntryTitle;
            oldEntry.EntryBody = entry.EntryBody;

            var newEntry = await _repository.UpdateBlogEntry(oldEntry);

            ProcessTags(entry.TagsString, newEntry);

            return Ok();
        }

        // PUT: api/BlogEntry/UpdateComment
        [HttpPut("UpdateComment")]
        public async Task<ActionResult> UpdateComment(Comment c)
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

            oldC.CommentBody = c.CommentBody;

            await _repository.UpdateComment(oldC);

            return Ok();
        }

        // DELETE: api/BlogEntry/DeleteEntry/{entryId}
        [HttpDelete("DeleteEntry/{entryId:int}")]
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
        [HttpDelete("DeleteComment/{commentId:int}")]
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

        // GET: api/BlogEntry/GetEntriesByTag/{tagId}
        [HttpGet("GetEntriesByTag/{tagId:int}")]
        public async Task<ActionResult<IEnumerable<BlogEntry>>> GetEntriesByTag([FromRoute] int tagId)
        {
            var dbentries = await _repository.GetBlogEntriesByTagId(tagId);

            // Workaround for passing recursive data into json
            List<BlogEntry> entries = new();
			foreach ( BlogEntry e in dbentries)
            {
                entries.Add(CopyBlogEntry(e));
            }

            return Ok(entries);
        }

        // Helpers:

        private async void ProcessTags(string tagsString, BlogEntry entry)
        {
            if (tagsString.IsNullOrEmpty())
            {
                return;
            }
            string[] tags = tagsString.Split(' ');
            List<string> validTags = new();
            bool tagIsValid;

            foreach (string tag in tags)
            {
                if (validateTag(tag))
                {
                    validTags.Add(tag);
                }
            }

            foreach (string tag in validTags)
            {
                await _repository.AddTagIfNewTag(tag);
            }
            await _repository.AddTagsToEntry(entry, validTags);
        }

        private bool validateTag(string tag)
        {
            if (!tag.IsNullOrEmpty())
            {
                if (tag[0] == '#' || tag.Length < 2) // first character must be a #, and the tag must contain at least 1 character
                {
                    for (int i = 1; i < tag.Length; i++) //start on index 1
                    {
                        if (!char.IsLetterOrDigit(tag[i]))
                        {
                            return false;
                        }
                    }
                    return true; // if all but the first character is alphanumeric, validate
                }
            }
            return false;
        }

        private BlogEntry CopyBlogEntry(BlogEntry oldEntry)
        {
            BlogEntry newEntry = new()
            {
                BlogEntryId = oldEntry.BlogEntryId,
                BlogId = oldEntry.BlogId,
                EntryTitle = oldEntry.EntryTitle,
                EntryBody = oldEntry.EntryBody,
            };

            foreach (Tag t in oldEntry.Tags)
            {
                newEntry.Tags.Add(new Tag() { TagId = t.TagId, TagName = t.TagName });
            }

            return newEntry;
        }

        private string GetUserIdFromLoggedInUser()
        {
            ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
            return identity.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value; //"NameIdentifier").Value;
        }
    }
}
