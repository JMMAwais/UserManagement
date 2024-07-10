using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.ViewModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UserManagement.Controllers
{

    public class ManageRollController : Controller
    {
        private readonly ApplicationDbContext _db;
        RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        public ManageRollController(ApplicationDbContext db, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AddRoll()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRoll(RollVM roll)
        {
            if (ModelState.IsValid)
            {
                var rollName = roll.RollName;
                await _roleManager.CreateAsync(new IdentityRole(roll.RollName));
                return RedirectToAction("Index", "Home");
            }

            return View(roll);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditRoll(string Id)
        {

            var roll = await _roleManager.FindByIdAsync(Id);

            return View(roll);
        }

        [HttpPost]
        public async Task<IActionResult> EditRoll(IdentityRole model)
        {
            var roll = await _roleManager.FindByIdAsync(model.Id);
            if (roll.Name == "SuperAdmin" || roll.Name == "Super Admin")
            {
                return View();
            }
            roll.Name = model.Name;
            await _roleManager.UpdateAsync(roll);
            return RedirectToAction("GetAllRoles");
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRoll(string Id)
        {



            var roll = await _roleManager.FindByIdAsync(Id);
            if (roll.Name == "SuperAdmin" || roll.Name == "Super Admin")
            {
                ModelState.AddModelError("", $"{roll.Name} Cannot be Deleted!");
                return RedirectToAction("GetAllRoles");
            }
            var check = await _userManager.GetUsersInRoleAsync(roll.Name);
            if (check.Count > 0)
            {
                ViewBag.Role = "This role is assigned! you can't delete this role";
                return RedirectToAction("GetAllRoles");
            }

            await _roleManager.DeleteAsync(roll);
            return RedirectToAction("GetAllRoles");

        }


        public IActionResult UsersInRoles()
        {
            var result2 = _db.Users
            .Join(_db.UserRoles, u => u.Id, ur => ur.UserId, (u, ur) => new { u, ur })
            .Join(_db.Roles, ur => ur.ur.RoleId, r => r.Id, (ur, r) => new { ur, r })
            .ToList()
             .GroupBy(uv => new { uv.ur.u.UserName, uv.ur.u.Email, uv.ur.u.Name, uv.ur.u.Id }).Select(r => new UsersInRoleVM()
             {
                 UserId = r.Key.Id,
                 Name = r.Key.Name,
                 Username = r.Key.UserName,
                 Email = r.Key.Email,
                 Role = string.Join(",", r.Select(c => c.r.Name).ToArray())
             }).ToList();
            return View(result2);
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> AssignRoles(string Id)
        {

            // var user= await _userManager.FindByIdAsync(Id);
            AssignRolesVM model = new AssignRolesVM();
            model.UserId = Id;
            var roles = _roleManager.Roles.Select(r => new SelectListItem
            {
                Value = r.Id,
                Text = r.Name
            }).ToList();
            model.RolesList = roles;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRoles(AssignRolesVM model)
        {
            var user= await _userManager.FindByIdAsync(model.UserId);
            if (user == null )
            {
                return View(model);
            }

            var checkrole = await _userManager.GetRolesAsync(user);
            if (checkrole != null)
            {
                var result = await _userManager.AddToRoleAsync(user, model.RollName);
                if (result.Succeeded)
                {
                    return RedirectToAction("UsersInRoles");
                }
            }
            return View(model);


        }
    }
}
