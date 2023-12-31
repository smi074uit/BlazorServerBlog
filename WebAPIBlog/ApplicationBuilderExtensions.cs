﻿using Microsoft.AspNetCore.Identity;
using SharedModels.Entities;
using WebAPIBlog.Data;

namespace WebAPIBlog
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task<IApplicationBuilder> PrepareDatabase(this IApplicationBuilder app)
        {
            using var scopedServices = app.ApplicationServices.CreateScope();

            var serviceProvider = scopedServices.ServiceProvider;
            var db = serviceProvider.GetRequiredService<ApplicationDbContext>();

            //db.Database.Migrate();
            if (db.Blog.Any())
            {
                return app;
            }

            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var hasher = new PasswordHasher<IdentityUser>();

            // Users
            var user1 = new IdentityUser
            {
                UserName = "kc@uit.no",
                NormalizedUserName = "KC@UIT.NO",
                Email = "kc@uit.no",
                NormalizedEmail = "KC@UIT.NO",
                LockoutEnabled = false,
                EmailConfirmed = true,
            };
            user1.PasswordHash = hasher.HashPassword(user1, "123456");
            await userManager.CreateAsync(user1);
            user1 = await userManager.FindByEmailAsync(user1.Email);

            var user2 = new IdentityUser
            {
                UserName = "DEF@uit.no",
                NormalizedUserName = "DEF@UIT.NO",
                Email = "DEF@uit.no",
                NormalizedEmail = "DEF@UIT.NO",
                LockoutEnabled = false,
                EmailConfirmed = true,
            };
            user2.PasswordHash = hasher.HashPassword(user1, "123456");
            await userManager.CreateAsync(user2);
            user2 = await userManager.FindByEmailAsync(user2.Email);

            // Seeding

            // Blogs
            Blog blog1 = new Blog
            { BlogTitle = "Hammere og slikt", Description = "Alt om verktøy", Locked = false, Owner = user1 };
            Blog dbBlog1 = db.Blog.Add(blog1).Entity;
            Blog blog2 = new Blog
            { BlogTitle = "Biler og sånt", Description = null, Locked = false, Owner = user2 };
            Blog dbBlog2 = db.Blog.Add(blog2).Entity;
            Blog blog3 = new Blog
            { BlogTitle = "Mat og ting", Description = "Oppskrifter og midagsideer", Locked = true, Owner = user1 };
            Blog dbBlog3 = db.Blog.Add(blog3).Entity;

            await db.SaveChangesAsync();

            dbBlog1 = db.Blog.Entry(dbBlog1).Entity;
            dbBlog2 = db.Blog.Entry(dbBlog2).Entity;
            dbBlog3 = db.Blog.Entry(dbBlog3).Entity;

            // Tags
            Tag tag1 = new Tag
            { TagName = "#Verktøy" };
            Tag dbtag1 = db.Tag.Add(tag1).Entity;
            Tag tag2 = new Tag
            { TagName = "#Biler" };
            Tag dbtag2 = db.Tag.Add(tag2).Entity;
            Tag tag3 = new Tag
            { TagName = "#Mat" };
            Tag dbtag3 = db.Tag.Add(tag3).Entity;

            await db.SaveChangesAsync();

            dbtag1 = db.Tag.Entry(dbtag1).Entity;
            dbtag2 = db.Tag.Entry(dbtag2).Entity;
            dbtag3 = db.Tag.Entry(dbtag3).Entity;

            // BlogEntries
            BlogEntry entry1 = new BlogEntry
            { BlogId = dbBlog1.BlogId, EntryTitle = "Min Favoritt Hammer", EntryBody = "Den jeg har i stua" };
            BlogEntry dbEntry1 = db.BlogEntry.Add(entry1).Entity;
            BlogEntry entry2 = new BlogEntry
            { BlogId = dbBlog1.BlogId, EntryTitle = "Vinkelsliper", EntryBody = "Sliper i vinkel" };
            BlogEntry dbEntry2 = db.BlogEntry.Add(entry2).Entity;
            BlogEntry entry3 = new BlogEntry
            { BlogId = dbBlog2.BlogId, EntryTitle = "Bil i garasjen", EntryBody = "Er det best å ha bilen i garasjen på sommeren?" };
            BlogEntry dbEntry3 = db.BlogEntry.Add(entry3).Entity;
            BlogEntry entry4 = new BlogEntry
            { BlogId = dbBlog3.BlogId, EntryTitle = "Dagens Middag: Spaghetti", EntryBody = "Spaghetti med kjøttboller, masse oregano" };
            BlogEntry dbEntry4 = db.BlogEntry.Add(entry4).Entity;

            await db.SaveChangesAsync();

            // BlogEntryTag relation
            dbEntry1 = db.BlogEntry.Entry(dbEntry1).Entity;
            dbEntry2 = db.BlogEntry.Entry(dbEntry2).Entity;
            dbEntry3 = db.BlogEntry.Entry(dbEntry3).Entity;

            //List<BlogEntry> entries1 = new() { dbEntry1 };
            //List<BlogEntry> entries2 = new() { dbEntry1, dbEntry2 };
            //List<BlogEntry> entries3 = new() { dbEntry3 };

            //List<Tag> tags1 = new() { dbtag1, dbtag2 };
            //List<Tag> tags2 = new() { dbtag2 };
            //List<Tag> tags3 = new() { dbtag3 };

            dbEntry1.Tags.Add(dbtag1);
            dbEntry1.Tags.Add(dbtag2);
            dbEntry2.Tags.Add(dbtag2);
            dbEntry3.Tags.Add(dbtag3);

            dbEntry1 = db.BlogEntry.Update(dbEntry1).Entity;
            dbEntry2 = db.BlogEntry.Update(dbEntry2).Entity;
            dbEntry3 = db.BlogEntry.Update(dbEntry3).Entity;

            await db.SaveChangesAsync();

            dbtag1.Entries.Add(dbEntry1);
            dbtag2.Entries.Add(dbEntry1);
            dbtag2.Entries.Add(dbEntry2);
            dbtag3.Entries.Add(dbEntry3);

            dbtag1 = db.Tag.Update(dbtag1).Entity;
            dbtag2 = db.Tag.Update(dbtag2).Entity;
            dbtag3 = db.Tag.Update(dbtag3).Entity;

            await db.SaveChangesAsync();

            // Comments
            Comment c1 = new Comment
            { EntryId = dbEntry1.BlogEntryId, CommentBody = "Hva med den i garasjen?", Owner = user2 };
            db.Comment.Add(c1);
            Comment c2 = new Comment
            { EntryId = dbEntry1.BlogEntryId, CommentBody = "Har du ikke to i stua?", Owner = user2 };
            db.Comment.Add(c2);
            Comment c3 = new Comment
            { EntryId = dbEntry2.BlogEntryId, CommentBody = "Mente ikke å poste dette", Owner = user2 };
            db.Comment.Add(c3);
            Comment c4 = new Comment
            { EntryId = dbEntry3.BlogEntryId, CommentBody = "Den får masse støv hvis den står ute!", Owner = user1 };
            db.Comment.Add(c4);


            await db.SaveChangesAsync();

            return app;
        }
    }
}
