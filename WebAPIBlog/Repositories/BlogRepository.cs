using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedModels.Entities;
using System.Security.Principal;
using WebAPIBlog.Data;

namespace WebAPIBlog.Repositories
{
	public class BlogRepository : IBlogRepository
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly ApplicationDbContext _db;

		public BlogRepository(UserManager<IdentityUser> userManager, ApplicationDbContext db)
		{
			this._userManager = userManager;
			this._db = db;
		}

		public async Task<IEnumerable<Blog>> GetAll()
		{
			var blogs = _db.Blog.Include(p => p.Owner);
			return blogs;
		}

		public async Task<Blog> AddBlog(BlogDTO blogDTO, string ownerID)
		{
			Blog blog = new()
			{
				BlogTitle = blogDTO.BlogTitle,
				Description = blogDTO.Description,
				Locked = false,
			};
			var user = await _userManager.FindByIdAsync(ownerID);
			blog.Owner = (IdentityUser)user;

			_db.Blog.Add(blog);
			_db.SaveChanges();

			Blog newBlog = _db.Blog.Include(p => p.Owner).FirstOrDefault(x => x.OwnerId == user.Id);

			return newBlog;
		}


		public async Task<BlogEntry> AddBlogEntry(BlogEntry blogEntry)
		{
			_db.BlogEntry.Add(blogEntry);
			_db.SaveChanges();

			BlogEntry savedEntry = _db.BlogEntry.Entry(blogEntry).Entity;
			return savedEntry;
		}


		public async Task<Comment> AddComment(CommentDTO cDTO, string userID)
		{
			Comment cToSave = new()
			{
				EntryId = cDTO.EntryId,
				CommentBody = cDTO.CommentBody
			};

			var user = await _userManager.FindByIdAsync(userID);
			cToSave.Owner = (IdentityUser)user;

			_db.Comment.Add(cToSave);
			_db.SaveChanges();

			Comment savedC = _db.Comment.Entry(cToSave).Entity;
			return savedC;
		}


		public async Task<Blog> GetBlogById(int blogId)
		{
			Blog blog = _db.Blog.Include(p => p.Owner).FirstOrDefault(x => x.BlogId == blogId);
			return blog;
		}

		public async Task<Blog> GetBlogByUser(string userID)
		{
			var user = await _userManager.FindByIdAsync(userID);
			Blog newBlog = _db.Blog.Include(p => p.Owner).FirstOrDefault(x => x.OwnerId == user.Id);

			return newBlog;
		}

		public async Task<IdentityUser> GetBlogOwner(int blogId)
		{
			var blog = _db.Blog.Include(p => p.Owner).FirstOrDefault(x => x.BlogId == blogId);
			var user = await _userManager.FindByNameAsync(blog.Owner.UserName);

			return user;
		}

		public async Task<bool> isBlogLocked(int blogId)
		{
			Blog blog = _db.Blog.Find(blogId);
			return blog.Locked;
		}

		public async Task<bool> toggleBlogLock(string userID)
		{
			var blog = _db.Blog.Include(p => p.Owner).FirstOrDefault(x => x.OwnerId == userID);
			if (blog.Locked)
			{
				blog.Locked = false;
			}
			else
			{
				blog.Locked = true;
			}

			_db.Blog.Update(blog);
			_db.SaveChanges();
			return blog.Locked;
		}

		public async Task<List<BlogEntry>> GetBlogEntriesByBlogId(int id)
		{
			IQueryable<BlogEntry> entries = _db.BlogEntry
				.Include(b => b.Tags)
				.Where(b => b.BlogId == id);
			List<BlogEntry> res = entries.ToList();

			return res;
		}

		public async Task<BlogEntry> GetBlogEntryById(int id)
		{
			var entry = _db.BlogEntry
				//.Include(b => b.Tags)
				.FirstOrDefault(x => x.BlogEntryId == id);
			return entry;
		}

		public async Task<Comment> GetCommentById(int id)
		{
			var comment = _db.Comment.Include(p => p.Owner).FirstOrDefault(x => x.CommentId == id);
			return comment;
		}

		public async Task<List<Comment>> GetComments(List<BlogEntry> entries)
		{
			List<Comment> comments = new();
			foreach (BlogEntry b in entries)
			{
				var c = _db.Comment
					.Where(c => c.EntryId == b.BlogEntryId)
					.Include(p => p.Owner)
					.ToList();
				comments = comments.Concat(c).ToList();
			}
			return comments;
		}

		public async Task UpdateBlog(Blog blog)
		{
			_db.Blog.Update(blog);
			_db.SaveChanges();
		}

		public async Task UpdateBlogEntry(BlogEntry entry)
		{
			_db.BlogEntry.Update(entry);
			_db.SaveChanges();
		}

		public async Task UpdateComment(Comment comment)
		{
			_db.Comment.Update(comment);
			_db.SaveChanges();
		}

		public async Task DeleteBlogEntryById(int id)
		{
			BlogEntry entry = _db.BlogEntry.Find(id);

			List<Comment> comments = _db.Comment
				.Where(c => c.EntryId == id)
				.Include(p => p.Owner)
				.ToList();

			if (comments.Count > 0)
			{
				foreach (Comment c in comments)
				{
					_db.Comment.Remove(c);
				}
			}

			_db.BlogEntry.Remove(entry);
			_db.SaveChanges();
		}

		public async Task DeleteCommentById(int id)
		{
			Comment c = _db.Comment.Find(id);
			_db.Comment.Remove(c);
			_db.SaveChanges();
		}

		public async Task AddTagIfNewTag(string tag)
		{
			if (!_db.Tag.Any(t => t.TagName == tag))
			{
				Tag dbTag = new() { TagName = tag };
				_db.Tag.Add(dbTag);
				_db.SaveChanges();
			}
			return;
		}

		public async Task AddTagsToEntry(BlogEntry entry, List<string> tags)
		{
			List<Tag> tagsList = new List<Tag>();
			BlogEntry dbEntry = _db.BlogEntry.Entry(entry).Entity;
			foreach (string tag in tags)
			{
				tagsList.Add(_db.Tag.Single(t => t.TagName == tag));
			}

			dbEntry.Tags.Clear();
			foreach (Tag t in tagsList)
			{
				dbEntry.Tags.Add(t);
			}
			

			_db.BlogEntry.Update(dbEntry);
			_db.SaveChanges();
		}
	}
}
