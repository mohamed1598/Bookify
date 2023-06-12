using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Bookify.WEB.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Category>().Property(e => e.CreatedOn).HasDefaultValueSql("GetDate()");
            builder.Entity<Author>().Property(e => e.CreatedOn).HasDefaultValueSql("GetDate()");
            builder.Entity<Book>().Property(e => e.CreatedOn).HasDefaultValueSql("GetDate()");
            builder.Entity<BookCategory>().HasKey(e => new { e.BookId, e.CategoryId });
            base.OnModelCreating(builder);
        }
    }
}