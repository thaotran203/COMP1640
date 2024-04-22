using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebEnterprise_1640.Data;
using WebEnterprise_1640.Models;
using System.Text.Json;

namespace WebEnterprise_1640.MagazineController
{
    [Area("Coordinator")]
    public class MagazineSelectedController : Controller
    {
        private readonly ILogger<MagazineController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public MagazineSelectedController(ILogger<MagazineController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var userJson = HttpContext.Session.GetString("USER");
            UserModel user = null;
            if (userJson != null && userJson.Length > 0)
            {
                user = JsonSerializer.Deserialize<UserModel>(userJson);
            }
            if (user == null)
            {
                return Redirect("/Account/Login");
            }
            var userRole = _dbContext.UserRoles.FirstOrDefault(ur => ur.UserId == user.Id);
            if (userRole == null)
            {
                return Redirect("/Account/Login");
            }
            var role = _dbContext.Roles.FirstOrDefault(r => r.Id == userRole.RoleId);
            if (role == null)
            {
                return Redirect("/Account/Login");
            }
            if (role.Name.ToLower() != "coordinator")
            {
                return Redirect("/Account/Login");
            }
            ViewBag.User = user;
            var magazines = await _dbContext.Magazines.Include(m => m.Semester).ToListAsync();
            return View("Index", magazines);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
