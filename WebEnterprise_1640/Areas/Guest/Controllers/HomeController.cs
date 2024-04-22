using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebEnterprise_1640.Data;
using WebEnterprise_1640.Models;
using WebEnterprise_1640.Utility;

namespace WebEnterprise_1640.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)

        {
            _context = context;
        }

        [Route("[controller]/[action]")]
        public async Task<IActionResult> MainPage()
        {
            return View();
        }
        [Route("[controller]/[action]")]
        public async Task<IActionResult> Index(int? facilityIdSort = null, string? search = null)
        {
            var userJson = HttpContext.Session.GetString("USER");
            UserModel? user = null;
            if (userJson != null && userJson.Length > 0)
            {
                user = JsonSerializer.Deserialize<UserModel>(userJson);
            }
            string roleStr = "";
            if (user != null)
            {
                var userRole = _context.UserRoles.FirstOrDefault(ur => ur.UserId == user.Id);
                if (userRole != null)
                {
                    var role = _context.Roles.FirstOrDefault(r => r.Id == userRole.RoleId);
                    if (role != null)
                    {
                        roleStr = role.Name;
                    }
                }
            }
            GUIUtils.CheckNotification(_context);
            var magazines = _context.Magazines.ToList();
            FacultyModel? curFacility = null;
            if (magazines != null && magazines.Count > 0)
            {
                if (user != null)
                {
                    magazines = magazines.Where(m => m.FacultyId == user.FacultyId).ToList();
                }
                if (facilityIdSort != null)
                {
                    magazines = magazines.Where(m => m.FacultyId == facilityIdSort).ToList();
                    curFacility = _context.Faculties.FirstOrDefault(f => f.Id == facilityIdSort);
                }
                if (search != null)
                {
                    magazines = magazines.Where(m => m.Name.ToLower().Contains(search.ToLower())).ToList();
                }
                foreach (var magazine in magazines)
                {
                    magazine.Articles = _context.Articles.Where(a => a.MagazineId == magazine.Id && a.Status.ToLower() == "approved".ToLower()).ToList();
                    if (magazine.Articles != null && magazine.Articles.Count > 0)
                    {
                        foreach (var article in magazine.Articles)
                        {
                            article.Documents = _context.Documents.Where(d => d.ArticleId == article.Id).ToList();
                        }
                    }
                }
            }
            var facilities = _context.Faculties.ToList();
            return View(new HomeViewModel()
            {
                Magazines = magazines,
                FacultyIdSort = facilityIdSort,
                Search = search,
                Faculties = facilities,
                CurFacility = curFacility,
                User = user,
                UserRole = roleStr
            });
        }
    }
}
