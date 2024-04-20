using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using WebEnterprise_1640.Data;
using WebEnterprise_1640.Models.ViewModel;

namespace WebEnterprise_1640.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AccountController(ApplicationDbContext context)

        {
            _context = context;
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
                ViewBag.Err = "Email Sai Định Dạng";
            }
            else
            {
                var user = _context.Users.FirstOrDefault(x => x.Email == modelLogin.Username);
                if (user == null)
                {
                    ViewBag.Err = "Tài Khoản Không Tồn Tại";

                }
                if (user != null && user.PasswordHash != modelLogin.Password)
                {
                    ViewBag.Err = "Sai Mật Khẩu";

                }
                if (user != null && user.PasswordHash == modelLogin.Password)
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
                        }
                        if (role.RoleId == "5")
                        {
                            newRole = "Admin";
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
