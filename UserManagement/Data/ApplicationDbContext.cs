using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;

namespace UserManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }



        public DbSet<Product> Products { get; set; }

    //    protected override void OnModelCreating(ModelBuilder modelBuilder)
    //    {
    //        modelBuilder.Entity<IdentityRole>().HasData(
    //           new IdentityRole
    //        {
    //          Id =Guid.NewGuid().ToString(),
    //          Name = "Super Admin",
    //          NormalizedName = "SUPER ADMIN"
    //        }
    //      );

    //        modelBuilder.Entity<IdentityUser>().HasData(
    //     new IdentityUser
    //     {
    //         Id = Guid.NewGuid().ToString(),
    //         UserName = "SuperAdmin@gmail.com",
    //         Email = "SuperAdmin@gmail.com",
    //         PasswordHash=new PasswordHasher<IdentityUser>().HashPassword(null,"Admin@123")
    //     }
    //);
    //    }
    }
}
