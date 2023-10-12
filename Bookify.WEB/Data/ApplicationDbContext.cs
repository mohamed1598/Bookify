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
        public DbSet<Area> Areas { get; set; }
        public DbSet<Governerate> Governerates { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<Subscribtion> Subscribtions { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<RentalCopy> RentalCopies { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Category>().Property(e => e.CreatedOn).HasDefaultValueSql("GetDate()");
            builder.Entity<Author>().Property(e => e.CreatedOn).HasDefaultValueSql("GetDate()");
            builder.Entity<Book>().Property(e => e.CreatedOn).HasDefaultValueSql("GetDate()");
            builder.Entity<BookCategory>().HasKey(e => new { e.BookId, e.CategoryId });
            builder.HasSequence<int>("SerialNumber", schema: "shared").StartsAt(1000001);
            builder.Entity<BookCopy>().Property(e => e.CreatedOn).HasDefaultValueSql("GetDate()");
            builder.Entity<BookCopy>().Property(e => e.SerialNumber).HasDefaultValueSql("NEXT VALUE FOR shared.SerialNumber");
            builder.Entity<Rental>().Property(e => e.CreatedOn).HasDefaultValueSql("GetDate()");
            builder.Entity<Rental>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<RentalCopy>().HasQueryFilter(e => !e.Rental!.IsDeleted);
            builder.Entity<RentalCopy>().HasKey(e => new {e.RentalId,e.BookCopyId});

            var cascadeFKs = builder.Model.GetEntityTypes().SelectMany(t => t.GetForeignKeys())
                .Where(fk =>fk.DeleteBehavior == DeleteBehavior.Cascade &&!fk.IsOwnership);
            foreach(var item in cascadeFKs)
                item.DeleteBehavior = DeleteBehavior.Restrict;
            base.OnModelCreating(builder);
        }
    }
}