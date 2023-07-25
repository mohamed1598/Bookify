using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Bookify.WEB.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }
        public DbSet<BookCopy> BookCopies { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Category>().Property(e => e.CreatedOn).HasDefaultValueSql("GetDate()");
            builder.Entity<Author>().Property(e => e.CreatedOn).HasDefaultValueSql("GetDate()");
            builder.Entity<Book>().Property(e => e.CreatedOn).HasDefaultValueSql("GetDate()");
            builder.Entity<BookCategory>().HasKey(e => new { e.BookId, e.CategoryId });
            builder.HasSequence<int>("SerialNumber", schema: "shared").StartsAt(1000001);
            builder.Entity<BookCopy>().Property(e => e.CreatedOn).HasDefaultValueSql("GetDate()");
            builder.Entity<BookCopy>().Property(e => e.SerialNumber).HasDefaultValueSql("NEXT VALUE FOR shared.SerialNumber");
            base.OnModelCreating(builder);
        }
    }
}