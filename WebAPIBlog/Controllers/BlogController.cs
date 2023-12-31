﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
                Comments = comments
            };

            // Workaround for passing recursive data into json
            foreach (BlogEntry entry in entries)
            {
                viewData.BlogEntries.Add(CopyBlogEntry(entry));
            }

            return Ok(viewData);
        }

        // GET: api/Blog/GetTags
        [AllowAnonymous]
        [HttpGet("GetAllTags")]
        public async Task<ActionResult<IEnumerable<Tag>>> GetAllTags()
        {
            IEnumerable<Tag> res = await _repository.GetAllTags();
            return Ok(res);
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

        // GET: api/Blog/DoesUserHaveBlog
        [HttpGet("DoesUserHaveBlog")]
        public async Task<bool> DoesUserHaveBlog()
        {
            string userID = GetUserIdFromLoggedInUser();

            Blog? res = await _repository.GetBlogByUser(userID);

            return res != null;
        }

        // GET: api/Blog/GetBlogIdByUsername/{username}
        [HttpGet("GetBlogIdByUsername/{username}")]
        public async Task<ActionResult<int>> GetBlogIdByUsername([FromRoute]string username)
        {
            IdentityUser user = await _repository.GetUserByUsername(username);

            if (user == null)
            {
                return Ok(-1);
            }

            Blog blog = await _repository.GetBlogByUser(user.Id);

            if (blog == null)
            {
                return Ok(-2);
            }

            return Ok(blog.BlogId);
        }


        // Helpers:

        private string GetUserIdFromLoggedInUser()
        {
            ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
            return identity.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value; //"NameIdentifier").Value;
        }

        private BlogEntry CopyBlogEntry(BlogEntry oldEntry)
        {
            BlogEntry newEntry = new()
            {
                BlogEntryId = oldEntry.BlogEntryId,
                BlogId = oldEntry.BlogId,
                EntryTitle = oldEntry.EntryTitle,
                EntryBody = oldEntry.EntryBody
            };

            foreach (Tag t in oldEntry.Tags)
            {
                newEntry.Tags.Add(new Tag() { TagId = t.TagId, TagName = t.TagName });
            }

            return newEntry;
        }

    }
}

