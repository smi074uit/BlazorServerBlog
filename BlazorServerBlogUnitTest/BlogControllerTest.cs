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
    public class BlogControllerTest
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

        //GetBlogs
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

        //GetBlogEntries
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

		//GetAllTags
		[TestMethod]
		public void GetAllTags()
		{
			// Arrange
			_repository.Setup(x => x.GetAllTags())
				.ReturnsAsync(faketags);

			var controller = new BlogController(_repository.Object);
			// Act
			ActionResult<IEnumerable<Tag>> result = controller.GetAllTags().Result;
			IEnumerable<Tag> tagresult = (result.Result as OkObjectResult).Value as IEnumerable<Tag>;
			// Assert
			CollectionAssert.AllItemsAreInstancesOfType((ICollection)tagresult,
				typeof(Tag));
			Assert.IsNotNull(result, "View Result is null");
			var tags = tagresult as List<Tag>;
			Assert.AreEqual(3, tags.Count, "Got wrong number of tags");
		}

		//CreateBlog
		[TestMethod]
        public void AddBlogIsCalledWhenBlogIsCreated()
        {
            // Arrange
            _repository.Setup(x => x.AddBlog(It.IsAny<BlogDTO>(), It.IsAny<string>()));
            _repository.Setup(x => x.GetBlogByUser(It.IsAny<string>()))
                .ReturnsAsync(fakeblogs[0]);
            var controller = new BlogController(_repository.Object);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);
            // Act
            BlogDTO blog = new()
            {
                BlogTitle = "Hammere og slikt",
                Description = "Test",
            };

            var result = controller.CreateBlog(blog);
            // Assert
            _repository.VerifyAll();
            Assert.IsNotNull(result);
            // test på at save er kalt et bestemt antall ganger
            _repository.Verify(x => x.AddBlog(It.IsAny<BlogDTO>(), It.IsAny<string>()), Times.Exactly(1));
            _repository.Verify(x => x.GetBlogByUser(It.IsAny<string>()), Times.Exactly(1));
        }

		//UpdateBlog
		[TestMethod]
		public void UpdateBlog()
		{
			// Arrange
			_repository.Setup(x => x.GetBlogByUser(It.IsAny<string>()))
				.ReturnsAsync(fakeblogs[0]);
			_repository.Setup(x => x.UpdateBlog(It.IsAny<Blog>()));
			var controller = new BlogController(_repository.Object);
			controller.ControllerContext = MockHelpers.FakeControllerContext(true);
			// Act
			BlogDTO blog = new()
			{
				BlogTitle = "Hammere og slikt",
				Description = "Test",
			};

			var result = controller.UpdateBlog(blog);
			// Assert
			_repository.VerifyAll();
			Assert.IsNotNull(result);
			
			_repository.Verify(x => x.GetBlogByUser(It.IsAny<string>()), Times.Exactly(1));
			_repository.Verify(x => x.UpdateBlog(It.IsAny<Blog>()), Times.Exactly(1));
		}

		//ToggleBlogLock
		[TestMethod]
		public void ToggleBlogLock()
		{
			// Arrange
			_repository.Setup(x => x.toggleBlogLock(It.IsAny<string>()));
			var controller = new BlogController(_repository.Object);
			controller.ControllerContext = MockHelpers.FakeControllerContext(true);
			// Act
			var result = controller.ToggleBlogLock();
			// Assert
			_repository.VerifyAll();
			Assert.IsNotNull(result);

			_repository.Verify(x => x.toggleBlogLock(It.IsAny<string>()), Times.Exactly(1));
		}

		//DoesUserHaveBlog
		[TestMethod]
		public void DoesUserHaveBlog()
		{
			// Arrange
			_repository.Setup(x => x.GetBlogByUser(It.IsAny<string>()))
				.ReturnsAsync(fakeblogs[0]);
			var controller = new BlogController(_repository.Object);
			controller.ControllerContext = MockHelpers.FakeControllerContext(true);
			// Act
			var result = controller.DoesUserHaveBlog();
			// Assert
			_repository.VerifyAll();
			Assert.IsNotNull(result);
			Assert.IsTrue(result.Result);

			_repository.Verify(x => x.GetBlogByUser(It.IsAny<string>()), Times.Exactly(1));
		}

		//GetBlogIdByUsername
		[TestMethod]
		public void GetBlogIdByUsername()
		{
			// Arrange
			_repository.Setup(x => x.GetUserByUsername(It.IsAny<string>()))
				.ReturnsAsync(fakeusers[0]);
			_repository.Setup(x => x.GetBlogByUser(It.IsAny<string>()))
				.ReturnsAsync(fakeblogs[0]);
			var controller = new BlogController(_repository.Object);
			controller.ControllerContext = MockHelpers.FakeControllerContext(true);
			// Act
			var result = controller.GetBlogIdByUsername("username");
			// Assert
			_repository.VerifyAll();
			Assert.IsNotNull(result);

			_repository.Verify(x => x.GetUserByUsername(It.IsAny<string>()), Times.Exactly(1));
			_repository.Verify(x => x.GetBlogByUser(It.IsAny<string>()), Times.Exactly(1));
		}
	}
}
