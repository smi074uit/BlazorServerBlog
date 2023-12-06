using System.Collections;
using System.Reflection.Metadata;
using System.Security.Principal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SharedModels.Entities;
using WebAPIBlog.Controllers;
using WebAPIBlog.Repositories;

namespace BlazorServerBlogUnitTest
{
	[TestClass]
	public class BlogControllerTest
	{
		Mock<IBlogRepository> _repository;

		List<Blog> fakeblogs;
		List<BlogEntry> fakeentries;
		List<Comment> fakecomments;
		List<IdentityUser> fakeusers;

		[TestInitialize]
		public void SetupContext()
		{

			_repository = new Mock<IBlogRepository>();

			fakeusers = new List<IdentityUser>
			{
				new IdentityUser
				{
					UserName = "kc@uit.no",
					NormalizedUserName = "KC@UIT.NO",
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

			fakeentries = new List<BlogEntry>
			{
				new BlogEntry
					{ BlogEntryId = 1, BlogId = 1, EntryTitle = "Min Favoritt Hammer", EntryBody = "Den jeg har i stua" },
				new BlogEntry
					{ BlogEntryId = 2, BlogId = 1, EntryTitle = "Vinkelsliper", EntryBody = "Sliper i vinkel" },
				new BlogEntry
					{ BlogEntryId = 3, BlogId = 2, EntryTitle = "Bil i garasjen", EntryBody = "Er det best å ha bilen i garasjen på sommeren?" },
				new BlogEntry
					{ BlogEntryId = 4, BlogId = 3, EntryTitle = "Dagens Middag: Spaghetti", EntryBody = "Spaghetti med kjøttboller, masse oregano" }
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

		[TestMethod]
		public void IndexReturnsAllProducts()
		{
			// Arrange
			_repository.Setup(x => x.GetAll()).ReturnsAsync(fakeblogs);
			var controller = new BlogController(_repository.Object);
			// Act
			ActionResult<IEnumerable<Blog>> result = controller.GetBlogs().Result;
			IEnumerable<Blog> blogresult = (result.Result as OkObjectResult).Value as IEnumerable<Blog>;
			// Assert
			CollectionAssert.AllItemsAreInstancesOfType((ICollection)blogresult,
				typeof(Blog));
			Assert.IsNotNull(result, "View Result is null");
			var blogs = blogresult as List<Blog>;
			Assert.AreEqual(3, blogs.Count, "Got wrong number of products");
		}

		[TestMethod]
		public void IndexReturnsAllEntriesAndComments()
		{
			// Arrange
			_repository.Setup(x => x.GetBlogById(It.IsAny<int>()))
				.ReturnsAsync(fakeblogs[0]);
			_repository.Setup(x => x.GetBlogEntriesByBlogId(It.IsAny<int>()))
				.ReturnsAsync(fakeentries);
			_repository.Setup(x => x.GetComments(It.IsAny<List<BlogEntry>>()))
				.ReturnsAsync(fakecomments);


			var controller = new BlogController(_repository.Object);
			// Act
			ActionResult<BlogViewModel> result = controller.GetBlogEntries(1).Result;
			BlogViewModel model = (result.Result as OkObjectResult).Value as BlogViewModel;
			// Assert
			Assert.IsNotNull(result, "View Result is null");
			Assert.IsInstanceOfType(model, typeof(BlogViewModel));

			Assert.IsNotNull(model.Blog);
			Assert.IsNotNull(model.BlogEntries);
			Assert.IsNotNull(model.Comments);

			CollectionAssert.AllItemsAreInstancesOfType((ICollection)model.BlogEntries,
				typeof(BlogEntry));

			CollectionAssert.AllItemsAreInstancesOfType((ICollection)model.Comments,
				typeof(Comment));
		}

		//[TestMethod]
		//public void CreateShouldShowLoginViewFor_Non_AuthorizedUser()
		//{
		//	// Arrange
		//	var mockUserManager = MockHelpers.MockUserManager<IdentityUser>();
		//	var mockRepo = new Mock<IBlogRepository>();
		//	var controller = new BlogController(mockRepo.Object); //, mockUserManager.Object
		//	controller.ControllerContext = MockHelpers.FakeControllerContext(false);

		//	// Act
		//	var result = controller.Create() as ViewResult;

		//	// Assert
		//	Assert.IsNotNull(result);
		//	Assert.IsNull(result.ViewName);

		//}

		//[TestMethod]
		//public void AddBlogIsCalledWhenBlogIsCreated()
		//{
		//	// Arrange
		//	_repository.Setup(x => x.AddBlog(It.IsAny<Blog>(), It.IsAny<IPrincipal>()));
		//	var controller = new BlogController(_repository.Object);
		//	controller.ControllerContext = MockHelpers.FakeControllerContext(true);
		//	// Act
		//	Blog blog = new Blog
		//	{
		//		BlogId = 1,
		//		BlogTitle = "Hammere og slikt",
		//		Description = "Test",
		//		Locked = false,
		//		Owner = fakeusers[0]
		//	};

		//	var result = controller.Create(blog);
		//	var result1 = controller.Create();
		//	// Assert
		//	_repository.VerifyAll();
		//	Assert.IsNotNull(result1);
		//	Assert.IsNotNull(result);
		//	// test på at save er kalt et bestemt antall ganger
		//	_repository.Verify(x => x.AddBlog(It.IsAny<Blog>(), It.IsAny<IPrincipal>()), Times.Exactly(1));
		//}

		//[TestMethod]
		//public void CreateEntry()
		//{
		//	// Arrange
		//	_repository.Setup(x => x.GetBlogEntryViewModel())
		//		.Returns(new BlogEntryViewModel());
		//	_repository.Setup(x => x.AddBlogEntry(It.IsAny<BlogEntry>()));
		//	var controller = new BlogController(_repository.Object);
		//	controller.ControllerContext = MockHelpers.FakeControllerContext(true);

		//	// Act
		//	var result1 = controller.CreateBlogEntry(1);

		//	BlogEntryViewModel entry = new BlogEntryViewModel
		//	{
		//		BlogEntryId = 1,
		//		BlogId = 1,
		//		EntryTitle = "Min Favoritt Hammer",
		//		EntryBody = "Den jeg har i stua"
		//	};

		//	var result = controller.CreateBlogEntry(1, entry);

		//	// Assert
		//	_repository.VerifyAll();
		//	Assert.IsNotNull(result1);
		//	Assert.IsNotNull(result);

		//	_repository.Verify(x => x.AddBlogEntry(It.IsAny<BlogEntry>()), Times.Exactly(1));
		//}

		//[TestMethod]
		//public void CreateComment()
		//{
		//	// Arrange
		//	_repository.Setup(x => x.AddComment(It.IsAny<Comment>(), It.IsAny<IPrincipal>()));
		//	var controller = new BlogController(_repository.Object);
		//	controller.ControllerContext = MockHelpers.FakeControllerContext(true);

		//	// Act
		//	Comment c = new Comment
		//	{
		//		CommentId = 1,
		//		EntryId = 1,
		//		CommentBody = "Hva med den i garasjen?",
		//		Owner = fakeusers[1]
		//	};

		//	var result = controller.CreateComment(1, c);
		//	var result1 = controller.CreateComment(1);
		//	// Assert
		//	_repository.VerifyAll();
		//	Assert.IsNotNull(result1);
		//	Assert.IsNotNull(result);
		//	// test på at save er kalt et bestemt antall ganger
		//	_repository.Verify(x => x.AddComment(It.IsAny<Comment>(), It.IsAny<IPrincipal>()), Times.Exactly(1));
		//}

		//[TestMethod]
		//public void ToggleBlogLocked()
		//{
		//	_repository.Setup(x => x.toggleBlogLock(It.IsAny<int>()));
		//	var controller = new BlogController(_repository.Object);
		//	controller.ControllerContext = MockHelpers.FakeControllerContext(true);

		//	var result = controller.ToggleBlogLock(1);

		//	_repository.VerifyAll();
		//	Assert.IsNotNull(result);
		//}

		//[TestMethod]
		//public void EditEntry()
		//{
		//	_repository.Setup(x => x.GetBlogEntryById(It.IsAny<int>()))
		//		.Returns(fakeentries[0]);
		//	_repository.Setup(x => x.UpdateBlogEntry(It.IsAny<BlogEntry>()));
		//	var controller = new BlogController(_repository.Object);
		//	controller.ControllerContext = MockHelpers.FakeControllerContext(true);

		//	BlogEntry entry = new BlogEntry
		//	{
		//		BlogEntryId = 1,
		//		BlogId = 1,
		//		EntryTitle = "Min Favoritt Hammer",
		//		EntryBody = "Den jeg har i stua"
		//	};

		//	var result = controller.EditBlogEntry(1);
		//	var result1 = controller.EditBlogEntry(1, entry);
		//	// Assert
		//	_repository.VerifyAll();
		//	Assert.IsNotNull(result1);
		//	Assert.IsNotNull(result);
		//	_repository.Verify(x => x.UpdateBlogEntry(It.IsAny<BlogEntry>()), Times.Exactly(1));
		//}

		//[TestMethod]
		//public void EditComment()
		//{
		//	_repository.Setup(x => x.GetCommentById(It.IsAny<int>()))
		//		.Returns(fakecomments[0]);
		//	_repository.Setup(x => x.UpdateComment(It.IsAny<Comment>()));
		//	var controller = new BlogController(_repository.Object);
		//	controller.ControllerContext = MockHelpers.FakeControllerContext(true);

		//	Comment c = new Comment
		//	{
		//		CommentId = 1,
		//		EntryId = 1,
		//		CommentBody = "Hva med den i garasjen?",
		//		Owner = fakeusers[1]
		//	};

		//	var result = controller.EditComment(1);
		//	var result1 = controller.EditComment(1, c);
		//	// Assert
		//	_repository.VerifyAll();
		//	Assert.IsNotNull(result1);
		//	Assert.IsNotNull(result);
		//	_repository.Verify(x => x.UpdateComment(It.IsAny<Comment>()), Times.Exactly(1));
		//}

		//[TestMethod]
		//public void DeleteBlogEntry()
		//{
		//	_repository.Setup(x => x.GetBlogEntryById(It.IsAny<int>()))
		//		.Returns(fakeentries[0]);
		//	_repository.Setup(x => x.DeleteBlogEntryById(It.IsAny<int>()))
		//		.Returns(fakeentries[0]);
		//	var controller = new BlogController(_repository.Object);
		//	controller.ControllerContext = MockHelpers.FakeControllerContext(true);

		//	BlogEntry entry = new BlogEntry
		//	{
		//		BlogEntryId = 1,
		//		BlogId = 1,
		//		EntryTitle = "Min Favoritt Hammer",
		//		EntryBody = "Den jeg har i stua"
		//	};

		//	var result = controller.DeleteBlogEntry(1);
		//	var result1 = controller.DeleteBlogEntry(1, entry);

		//	// Assert
		//	_repository.VerifyAll();
		//	Assert.IsNotNull(result1);
		//	Assert.IsNotNull(result);
		//	_repository.Verify(x => x.DeleteBlogEntryById(It.IsAny<int>()), Times.Exactly(1));
		//}

		//[TestMethod]
		//public void DeleteComment()
		//{
		//	_repository.Setup(x => x.GetCommentById(It.IsAny<int>()))
		//		.Returns(fakecomments[0]);
		//	_repository.Setup(x => x.DeleteCommentById(It.IsAny<int>()))
		//		.Returns(fakecomments[0]);
		//	var controller = new BlogController(_repository.Object);
		//	controller.ControllerContext = MockHelpers.FakeControllerContext(true);

		//	Comment c = new Comment
		//	{
		//		CommentId = 1,
		//		EntryId = 1,
		//		CommentBody = "Hva med den i garasjen?",
		//		Owner = fakeusers[1]
		//	};

		//	var result = controller.DeleteComment(1);
		//	var result1 = controller.DeleteComment(1, c);

		//	// Assert
		//	_repository.VerifyAll();
		//	Assert.IsNotNull(result1);
		//	Assert.IsNotNull(result);
		//	_repository.Verify(x => x.DeleteCommentById(It.IsAny<int>()), Times.Exactly(1));
		//}
	}
}
