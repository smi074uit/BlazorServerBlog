using BlazorServerBlog.Pages.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharedModels.Entities;

namespace BlazorServerBlog.Services
{
	public class BlogEntryService : IBlogEntryService
	{
		private readonly IApiHelper api;

		public BlogEntryService(IApiHelper api)
		{
			this.api = api;
		}

		public async Task<int> CreateBlogEntry(BlogEntryDTO entryDTO)
		{
			var response = api.PostData<BlogEntryDTO>(entryDTO, "BlogEntry/CreateEntry");

			if (!response.IsSuccessStatusCode)
			{
				return -1;
			}

			string result = await response.Content.ReadAsStringAsync();

			int intResult = Int32.Parse(result);

			return intResult;
		}

		public async Task<int> CreateComment(CommentDTO cDTO)
		{
			var response = api.PostData<CommentDTO>(cDTO, "BlogEntry/CreateComment");

			if (!response.IsSuccessStatusCode)
			{
				return -1;
			}

			string result = await response.Content.ReadAsStringAsync();

			int intResult = Int32.Parse(result);

			return intResult;
		}

		public async Task<BlogEntry> GetEntry(int entryId)
		{
			var response = api.GetData("BlogEntry/GetEntry/" + entryId.ToString());

			if (!response.IsSuccessStatusCode)
			{
				return null;
			}

			BlogEntry result = await response.Content.ReadFromJsonAsync<BlogEntry>();

			return result;
		}

		public async Task<Comment> GetComment(int commentId)
		{
			var response = api.GetData("BlogEntry/GetComment/" + commentId.ToString());

			if (!response.IsSuccessStatusCode)
			{
				return null;
			}

			Comment result = await response.Content.ReadFromJsonAsync<Comment>();

			return result;
		}

		public async Task<bool> UpdateEntry(BlogEntry entry)
		{
			var response = api.PutData<BlogEntry>(entry, "BlogEntry/UpdateEntry");

			if (!response.IsSuccessStatusCode)
			{
				return false;
			}

			return true;
		}

		public async Task<bool> UpdateComment(Comment c)
		{
			var response = api.PutData<Comment>(c, "BlogEntry/UpdateComment");

			if (!response.IsSuccessStatusCode)
			{
				return false;
			}

			return true;
		}

		public async Task<bool> DeleteEntry(int entryId)
		{
			var response = api.DeleteData("BlogEntry/DeleteEntry/" + entryId.ToString());

			if (!response.IsSuccessStatusCode)
			{
				return false;
			}

			return true;
		}

		// DELETE: api/BlogEntry/DeleteComment/{commentId}
		[HttpDelete("DeleteComment/{commentId}")]
		public async Task<bool> DeleteComment(int commentId)
		{
			var response = api.DeleteData("BlogEntry/DeleteComment/" + commentId.ToString());

			if (!response.IsSuccessStatusCode)
			{
				return false;
			}

			return true;
		}
	}
}
