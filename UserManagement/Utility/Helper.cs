using Microsoft.AspNetCore.Mvc.Rendering;
using UserManagement.Data;

namespace UserManagement.Utility
{
    public  class Helper
    {
        private readonly ApplicationDbContext _db;
        public Helper(ApplicationDbContext db)
        {
                _db = db;
        }

        public  SelectListItem GetRolesForDropDown()
        {
            
            var roles = _db.Roles.ToList();
            return new SelectListItem
            {
                Value = roles.FirstOrDefault().Id,
                Text = roles.FirstOrDefault().Name
            };
        }
    }
}
