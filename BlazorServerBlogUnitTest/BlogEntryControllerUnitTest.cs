using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SharedModels.Entities;
using System.Collections;
using System.Reflection.Metadata;
using WebAPIBlog.Controllers;
using WebAPIBlog.Repositories;

namespace BlazorServerBlogUnitTest
{
	[TestClass]
	public class BlogEntryControllerTest
	{
		Mock<IBlogRepository> _repository;

		List<Blog> fakeblogs;
		List<BlogEntry> fakeentries;
		List<Comment> fakecomments;
		List<IdentityUser> fakeusers;
		List<Tag> faketags;

		[TestInitialize]
		public void SetupContext()
		{

			_repository = new Mock<IBlogRepository>();

			fakeusers = new List<IdentityUser>
			{
				new IdentityUser
				{
					Id = "userId",
					UserName = "username",
					NormalizedUserName = "USERNAME",
					Email = "kc@uit.no",
					NormalizedEmail = "KC@UIT.NO",
					LockoutEnabled = false,
					EmailConfirmed = true
				},
				new IdentityUser
				{
					UserName = "DEF@uit.no",
					NormalizedUserName = "DEF@UIT.NO",
					Email = "DEF@uit.no",
					NormalizedEmail = "DEF@UIT.NO",
					LockoutEnabled = false,
					EmailConfirmed = true
				}
			};

			fakeblogs = new List<Blog>
			{
				new Blog
					{ BlogId = 1, BlogTitle = "Hammere og slikt", Description = "Test", Locked = false, Owner = fakeusers[0] },

				new Blog
					{ BlogId = 2, BlogTitle = "Biler og sånt", Description = null, Locked = false, Owner = fakeusers[1] },

				new Blog
					{ BlogId = 3, BlogTitle = "Mat og ting", Description = "Test", Locked = true, Owner = fakeusers[0] },
			};

			faketags = new List<Tag>
			{
				new Tag
				{ TagId = 1, TagName = "#Verktøy" },
				new Tag
				{ TagId = 2, TagName = "#Biler" },
				new Tag
				{ TagId = 3, TagName = "#Mat" },
			};

			fakeentries = new List<BlogEntry>
			{
				new BlogEntry
					{ BlogEntryId = 1, BlogId = 1, EntryTitle = "Min Favoritt Hammer", EntryBody = "Den jeg har i stua", Tags = new List<Tag> { faketags[0], faketags[1] } },
				new BlogEntry
					{ BlogEntryId = 2, BlogId = 1, EntryTitle = "Vinkelsliper", EntryBody = "Sliper i vinkel", Tags = new List<Tag> { faketags[0] } },
				new BlogEntry
					{ BlogEntryId = 3, BlogId = 2, EntryTitle = "Bil i garasjen", EntryBody = "Er det best å ha bilen i garasjen på sommeren?", Tags = new List<Tag> { faketags[1] } },
				new BlogEntry
					{ BlogEntryId = 4, BlogId = 3, EntryTitle = "Dagens Middag: Spaghetti", EntryBody = "Spaghetti med kjøttboller, masse oregano", Tags = new List<Tag> { faketags[2] } }
			};

			fakecomments = new List<Comment>
			{
				new Comment
					{ CommentId = 1, EntryId = 1, CommentBody = "Hva med den i garasjen?", Owner = fakeusers[1] },
				new Comment
					{ CommentId = 2, EntryId = 1, CommentBody = "Har du ikke to i stua?", Owner = fakeusers[1] },
				new Comment
					{ CommentId = 3, EntryId = 2, CommentBody = "Mente ikke å poste dette", Owner = fakeusers[1] },
				new Comment
				{ CommentId = 4, EntryId = 3, CommentBody = "Den får masse støv hvis den står ute!", Owner = fakeusers[0] }
			};
		}

		//CreateBlogEntry
		[TestMethod]
		public void CreateBlogEntry()
		{
			// Arrange 
			_repository.Setup(x => x.GetBlogByUser(It.IsAny<string>()))
				.ReturnsAsync(fakeblogs[0]);
			_repository.Setup(x => x.AddBlogEntry(It.IsAny<BlogEntry>()))
				.ReturnsAsync(fakeentries[0]);
			var controller = new BlogEntryController(_repository.Object);
			controller.ControllerContext = MockHelpers.FakeControllerContext(true);
			// Act
			BlogEntryDTO entry = new()
			{
				BlogId = 1,
				EntryTitle = "title",
				EntryBody = "body"
			};

			var result = controller.CreateBlogEntry(entry);
			// Assert
			_repository.VerifyAll();
			Assert.IsNotNull(result);

			_repository.Verify(x => x.GetBlogByUser(It.IsAny<string>()), Times.Exactly(1));
			_repository.Verify(x => x.AddBlogEntry(It.IsAny<BlogEntry>()), Times.Exactly(1));
		}

		//CreateComment
		[TestMethod]
		public void CreateComment()
		{
			// Arrange 
			_repository.Setup(x => x.GetBlogEntryById(It.IsAny<int>()))
				.ReturnsAsync(fakeentries[0]);
			_repository.Setup(x => x.GetBlogById(It.IsAny<int>()))
				.ReturnsAsync(fakeblogs[0]);
			_repository.Setup(x => x.AddComment(It.IsAny<CommentDTO>(), It.IsAny<string>()))
				.ReturnsAsync(fakecomments[0]);
			var controller = new BlogEntryController(_repository.Object);
			controller.ControllerContext = MockHelpers.FakeControllerContext(true);
			// Act
			CommentDTO c = new()
			{
				EntryId = 1,
				CommentBody = "body"
			};

			var result = controller.CreateComment(c);
			// Assert
			_repository.VerifyAll();
			Assert.IsNotNull(result);

			_repository.Verify(x => x.GetBlogEntryById(It.IsAny<int>()), Times.Exactly(1));
			_repository.Verify(x => x.GetBlogById(It.IsAny<int>()), Times.Exactly(1));
			_repository.Verify(x => x.AddComment(It.IsAny<CommentDTO>(), It.IsAny<string>()), Times.Exactly(1));
		}

		// GetEntry
		[TestMethod]
		public void GetEntry()
		{
			// Arrange 
			_repository.Setup(x => x.GetBlogEntryById(It.IsAny<int>()))
				.ReturnsAsync(fakeentries[0]);
			var controller = new BlogEntryController(_repository.Object);
			controller.ControllerContext = MockHelpers.FakeControllerContext(true);
			// Act
			var result = controller.GetEntry(1);
			// Assert
			_repository.VerifyAll();
			Assert.IsNotNull(result);

			_repository.Verify(x => x.GetBlogEntryById(It.IsAny<int>()), Times.Exactly(1));
		}

		// GetComment
		[TestMethod]
		public void GetComment()
		{
			// Arrange 
			_repository.Setup(x => x.GetCommentById(It.IsAny<int>()))
				.ReturnsAsync(fakecomments[0]);
			var controller = new BlogEntryController(_repository.Object);
			controller.ControllerContext = MockHelpers.FakeControllerContext(true);
			// Act
			var result = controller.GetComment(1);
			// Assert
			_repository.VerifyAll();
			Assert.IsNotNull(result);

			_repository.Verify(x => x.GetCommentById(It.IsAny<int>()), Times.Exactly(1));
		}

		// UpdateEntry
		[TestMethod]
		public void UpdateEntry()
		{
			// Arrange 
			BlogEntry blogEntry = new()
			{
				BlogEntryId = 1,
				BlogId = 1,
				EntryTitle = "Title",
				EntryBody = "Body"
			};

			_repository.Setup(x => x.GetBlogEntryById(It.IsAny<int>()))
				.ReturnsAsync(blogEntry);
			_repository.Setup(x => x.GetBlogOwner(It.IsAny<int>()))
				.ReturnsAsync(fakeusers[0]);
			_repository.Setup(x => x.UpdateBlogEntry(It.IsAny<BlogEntry>()))
				.ReturnsAsync(fakeentries[0]);
			var controller = new BlogEntryController(_repository.Object);
			controller.ControllerContext = MockHelpers.FakeControllerContext(true);
			// Act
			BlogEntryDTO entry = new()
			{
				BlogId = 1,
				EntryTitle = "title",
				EntryBody = "body"
			};

			var result = controller.UpdateEntry(entry);
			// Assert
			_repository.VerifyAll();
			Assert.IsNotNull(result);

			_repository.Verify(x => x.GetBlogEntryById(It.IsAny<int>()), Times.Exactly(1));
			_repository.Verify(x => x.GetBlogOwner(It.IsAny<int>()), Times.Exactly(1));
			_repository.Verify(x => x.UpdateBlogEntry(It.IsAny<BlogEntry>()), Times.Exactly(1));
		}

		// UpdateComment
		[TestMethod]
		public void UpdateComment()
		{
			// Arrange 
			Comment c = new()
			{
				CommentId = 1,
				EntryId = 1,
				CommentBody = "body",
				Owner = fakeusers[0],
				OwnerId = "userId"
			};

			_repository.Setup(x => x.GetCommentById(It.IsAny<int>()))
				.ReturnsAsync(c);
			_repository.Setup(x => x.UpdateComment(It.IsAny<Comment>()));
			var controller = new BlogEntryController(_repository.Object);
			controller.ControllerContext = MockHelpers.FakeControllerContext(true);
			// Act
			Comment cDTO = new()
			{
				CommentId = 1,
				EntryId = 1,
				CommentBody = "Body2",
				Owner = fakeusers[0],
				OwnerId = "userId"
			};

			var result = controller.UpdateComment(cDTO);
			// Assert
			_repository.VerifyAll();
			Assert.IsNotNull(result);

			_repository.Verify(x => x.GetCommentById(It.IsAny<int>()), Times.Exactly(1));
			_repository.Verify(x => x.UpdateComment(It.IsAny<Comment>()), Times.Exactly(1));
		}

		// DeleteEntry
		[TestMethod]
		public void DeleteEntry()
		{
			// Arrange 
			_repository.Setup(x => x.GetBlogEntryById(It.IsAny<int>()))
				.ReturnsAsync(fakeentries[0]);
			_repository.Setup(x => x.GetBlogOwner(It.IsAny<int>()))
				.ReturnsAsync(fakeusers[0]);
			_repository.Setup(x => x.DeleteBlogEntryById(It.IsAny<int>()));
			var controller = new BlogEntryController(_repository.Object);
			controller.ControllerContext = MockHelpers.FakeControllerContext(true);
			// Act
			var result = controller.DeleteEntry(1);
			// Assert
			_repository.VerifyAll();
			Assert.IsNotNull(result);

			_repository.Verify(x => x.GetBlogEntryById(It.IsAny<int>()), Times.Exactly(1));
			_repository.Verify(x => x.GetBlogOwner(It.IsAny<int>()), Times.Exactly(1));
			_repository.Verify(x => x.DeleteBlogEntryById(It.IsAny<int>()), Times.Exactly(1));
		}

		// DeleteComment
		[TestMethod]
		public void DeleteComment()
		{
			// Arrange 
			Comment c = new()
			{
				CommentId = 1,
				EntryId = 1,
				CommentBody = "body",
				Owner = fakeusers[0],
				OwnerId = "userId"
			};

			_repository.Setup(x => x.GetCommentById(It.IsAny<int>()))
				.ReturnsAsync(c);
			_repository.Setup(x => x.DeleteCommentById(It.IsAny<int>()));
			var controller = new BlogEntryController(_repository.Object);
			controller.ControllerContext = MockHelpers.FakeControllerContext(true);
			// Act
			var result = controller.DeleteComment(1);
			// Assert
			_repository.VerifyAll();
			Assert.IsNotNull(result);

			_repository.Verify(x => x.GetCommentById(It.IsAny<int>()), Times.Exactly(1));
			_repository.Verify(x => x.DeleteCommentById(It.IsAny<int>()), Times.Exactly(1));
		}

		// GetEntriesByTag
		[TestMethod]
		public void GetEntriesByTag()
		{
			// Arrange 
			_repository.Setup(x => x.GetBlogEntriesByTagId(It.IsAny<int>()))
				.ReturnsAsync(fakeentries);
			var controller = new BlogEntryController(_repository.Object);
			controller.ControllerContext = MockHelpers.FakeControllerContext(true);
			// Act
			var result = controller.GetEntriesByTag(1).Result;
			IEnumerable<BlogEntry> blogresult = (result.Result as OkObjectResult).Value as IEnumerable<BlogEntry>;
			// Assert
			_repository.VerifyAll();
			Assert.IsNotNull(result);

			CollectionAssert.AllItemsAreInstancesOfType((ICollection)blogresult,
				typeof(BlogEntry));
			Assert.IsNotNull(result, "View Result is null");
			var entries = blogresult as List<BlogEntry>;
			Assert.AreEqual(4, entries.Count, "Got wrong number of entries");

			_repository.Verify(x => x.GetBlogEntriesByTagId(It.IsAny<int>()), Times.Exactly(1));
		}
	}
}
