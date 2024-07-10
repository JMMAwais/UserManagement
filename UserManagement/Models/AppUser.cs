using Microsoft.AspNetCore.Identity;

namespace UserManagement.Models
{
    public class AppUser:IdentityUser
    {
        public string  Name { get; set; }
    }
}
