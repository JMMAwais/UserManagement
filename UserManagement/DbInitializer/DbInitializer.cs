using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using UserManagement.Data;
using UserManagement.Models;

namespace UserManagement.DbInitializer
{
    public class DbInitializer:IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DbInitializer(ApplicationDbContext db, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
                 _db = db;
                 _userManager = userManager;
                 _roleManager = roleManager; 
        }

        public async void Initialize()
        {
            try
            {
                if(_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
              
            }
            if (_db.Roles.Any(x => x.Name == "SuperAdmin")) return;
            if (! _roleManager.RoleExistsAsync("SuperAdmin").GetAwaiter().GetResult())
            {
                  _roleManager.CreateAsync(new IdentityRole("SuperAdmin")).GetAwaiter().GetResult();
                  _userManager.CreateAsync(new AppUser
                {
                    UserName = "SuperAdmin@gmail.com",
                    Email = "SuperAdmin@gmail.com",
                    Name = "SuperUser"
                }, "Admin@123").GetAwaiter().GetResult();

                AppUser user = _userManager.Users.FirstOrDefault(x => x.Email== "SuperAdmin@gmail.com");
               _userManager.AddToRoleAsync(user, "SuperAdmin").GetAwaiter().GetResult();
            }
        }
    }
}
