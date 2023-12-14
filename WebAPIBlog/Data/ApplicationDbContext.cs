using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SharedModels.Entities;

namespace WebAPIBlog.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        { }

        public DbSet<Blog> Blog { get; set; }
        public DbSet<Tag> Tag { get; set; }
        public DbSet<BlogEntry> BlogEntry { get; set; }
        public DbSet<Comment> Comment { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Blog>().ToTable("Blog");

            builder.Entity<Tag>().ToTable("Tags");

            builder.Entity<BlogEntry>().ToTable("BlogEntry");

            builder.Entity<Comment>().ToTable("Comment");


            builder.Entity<IdentityUser>(entity =>
            {
                entity.Property(m => m.Id).HasMaxLength(127);
                entity.Property(r => r.EmailConfirmed)
                .HasConversion(new BoolToZeroOneConverter<Int16>());
                entity.Property(m => m.LockoutEnabled)
                .HasConversion(new BoolToZeroOneConverter<Int16>());
                entity.Property(m => m.PhoneNumberConfirmed)
                .HasConversion(new BoolToZeroOneConverter<Int16>());
                entity.Property(m => m.TwoFactorEnabled)
                .HasConversion(new BoolToZeroOneConverter<Int16>());
            });
            builder.Entity<IdentityRole>(entity => entity.Property(m => m.Id).HasMaxLength(127));
            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.Property(m => m.LoginProvider).HasMaxLength(127);
                entity.Property(m => m.ProviderKey).HasMaxLength(127);
                entity.Property(m => m.UserId).HasMaxLength(127);
            });
            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.Property(m => m.UserId).HasMaxLength(127);
                entity.Property(m => m.RoleId).HasMaxLength(127);
            });
            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.Property(m => m.UserId).HasMaxLength(127);
                entity.Property(m => m.LoginProvider).HasMaxLength(127);
                entity.Property(m => m.Name).HasMaxLength(127);
            });
        }

    }
}
