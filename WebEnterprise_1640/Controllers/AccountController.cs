using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using WebEnterprise_1640.Data;
using WebEnterprise_1640.Models;

namespace WebEnterprise_1640.Controllers
{
    public class AccountController :Controller
    {
        private readonly ApplicationDbContext _context;
        public AccountController(ApplicationDbContext context)

        {
            _context = context;
        }
        public IActionResult Login(string url)
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", url);
            }
            return View();
        }

        [HttpPost]
        [Route("[controller]/[action]")]
        public async Task<IActionResult> Login(Login modelLogin)
        {
            if (!modelLogin.Username.Contains("@gmail.com"))
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

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                     new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() { new Claim(ClaimTypes.Name, ""), new Claim(ClaimTypes.Role, "Admin") },
                    CookieAuthenticationDefaults.AuthenticationScheme)), new AuthenticationProperties() { AllowRefresh = true, });
                    Response.Cookies.Append("userInfo", "Admin");
                    var admin = "Admin";
                    if (admin == "Admin")
                    {
                        return RedirectToAction("Index", "Magazines");
                    }

                    return RedirectToAction("Index", "Home");
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

            return RedirectToAction("Index", "Home");
        }
    }
}
