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
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Category>().Property(e => e.CreatedOn).HasDefaultValueSql("GetDate()");
            builder.Entity<Author>().Property(e => e.CreatedOn).HasDefaultValueSql("GetDate()");
            base.OnModelCreating(builder);
        }
    }
}