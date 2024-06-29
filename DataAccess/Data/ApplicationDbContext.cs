using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                    new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                    new Category { Id = 2, Name = "Fiction", DisplayOrder = 1 },
                    new Category { Id = 3, Name = "Science", DisplayOrder = 1 }
            );

            modelBuilder.Entity<Product>().HasData(
                   new Product { 
                       Id = 1, 
                       Name = "Harry Potter", 
                       Author = "J.K. Rolling", 
                       Description = "Mysterious journey of young boy Harry Potter",
                       ISBN = "99999999",
                       ListPrice = 50,
                       ImgUrl = null
                   }
            );
        }
    }
}
