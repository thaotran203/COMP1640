using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using WebEnterprise_1640.Data;
using WebEnterprise_1640.Models;
using WebEnterprise_1640.Models.ViewModel;

namespace WebEnterprise_1640.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UserModel> _userManager;
        public AccountController(ApplicationDbContext context, UserManager<UserModel> userManager)

        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("[controller]/[action]")]
        public async Task<IActionResult> Login(Login modelLogin)
        {
            if (!modelLogin.Username.Contains("@"))
            {
                ViewBag.Err = "Email Invalid Format!";
            }
            else
            {
                var user = await _userManager.FindByEmailAsync(modelLogin.Username);
                if (user == null)
                {
                    ViewBag.Err = "Account Does Not Exist!";

                }
                // Check if the provided password matches the hashed password
                var passwordValid = await _userManager.CheckPasswordAsync(user, modelLogin.Password);
                if (!passwordValid)
                {
                    ViewBag.Err = "Wrong Password!";

                }
                else
                {
                    var role = _context.UserRoles.FirstOrDefault(x => x.UserId == user.Id);
                    var newRole = "";
                    HttpContext.Session.SetString("USER", JsonSerializer.Serialize(user));
                    if (role != null)
                    {
                        if (role.RoleId == "1")
                        {
                            newRole = "Guest";
                        }
                        if (role.RoleId == "2")
                        {
                            newRole = "Student";
                        }
                        if (role.RoleId == "3")
                        {
                            newRole = "Coordinator";
                            return RedirectToAction("Index", "Magazine", new { area = "Coordinator" });
                        }
                        if (role.RoleId == "4")
                        {
                            newRole = "Manager";
                            return RedirectToAction("Index", "Dashboard", new { area = "Manager" });
                        }
                        if (role.RoleId == "5")
                        {
                            newRole = "Admin";
                            return RedirectToAction("Index", "Faculty", new { area = "Admin" });
                        }
                    }
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                     new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() { new Claim(ClaimTypes.Name, ""), new Claim(ClaimTypes.Role, newRole) },
                    CookieAuthenticationDefaults.AuthenticationScheme)), new AuthenticationProperties() { AllowRefresh = true, });
                    Response.Cookies.Append("userInfo", newRole);
                    return Redirect("/Home/Index");
                }
            }
            return View();
        }


        [HttpGet]
        [Route("[controller]/[action]")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("userInfo");
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
