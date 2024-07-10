using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Net.WebSockets;
using System.Security.Claims;
using UserManagement.Data;
using UserManagement.Migrations;
using UserManagement.Models;
using UserManagement.Utility;
using UserManagement.ViewModel;

namespace UserManagement.Controllers
{
    public class AccountController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly Helper _helper;

        public AccountController(ApplicationDbContext db,
            UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager, Helper helper)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _helper = helper;
        }

        public IActionResult Login()
        {
            return View();
        }


           [Authorize(Roles="Admin, SuperAdmin")]
            public ActionResult UsersWithRoles()
        {
                    var result2 = _db.Users
                    .Join(_db.UserRoles, u => u.Id, ur => ur.UserId, (u, ur) => new { u, ur })
                    .Join(_db.Roles, ur => ur.ur.RoleId, r => r.Id, (ur, r) => new { ur, r })
                    .ToList()
                     .GroupBy(uv => new { uv.ur.u.UserName, uv.ur.u.Email, uv.ur.u.Name }).Select(r => new UsersInRoleVM()
                       {
                       Name = r.Key.Name,
                       Username = r.Key.UserName,
                       Email = r.Key.Email,
                       Role = string.Join(",", r.Select(c => c.r.Name).ToArray())
                       }).ToList();
                        return View(result2);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("message", "Invalid Email Address!");
                }
                if (await _userManager.CheckPasswordAsync(user, model.Password) == false)
                {
                    ModelState.AddModelError("message", "Invalid Password!");
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    ModelState.AddModelError("message", "Invalid UserName or Password");
                }
            }
            return View();
        }


        public async Task<IActionResult> Register()
        {
          

            var model = new RegisterViewModel();
            var roles = _roleManager.Roles.Select(r => new SelectListItem
            {                
                Value = r.Name,
                Text = r.Name
            }).ToList();
            model.RolesList = roles;
            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
          
                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                };

                var result= await _userManager.CreateAsync(user,model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.RollName);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Home");
                }
            

            return View();

        }



        [HttpPost]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult  ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            if (token != null)
            {
                return RedirectToAction("ResetPassword", new {UserEmail=user.Email, Token=token});
            }

            return View();
        }


        [HttpGet]
        public IActionResult ResetPassword(string UserEmail, string Token)
        {
            // If password reset token or email is null, most likely the
            // user tried to tamper the password reset link
            if (Token == null || UserEmail == null)
            {
                ViewBag.ErrorTitle = "Invalid Password Reset Token";
                ViewBag.ErrorMessage = "The Link is Expired or Invalid";
                return View("Error");
            }
            else
            {
                PasswordResetVM model = new PasswordResetVM();
                model.Token = Token;
                model.Email = UserEmail;
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(PasswordResetVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ResetPasswordConfirmation", "Account");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
          
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            // Display validation errors if model state is not valid
            return View(model);
        }

        public IActionResult ResetPasswordConfirmation()
        {

            return View();
        }

    } 
}