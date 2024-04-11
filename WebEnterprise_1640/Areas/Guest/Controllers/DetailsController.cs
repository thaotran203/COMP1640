using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebEnterprise_1640.Data;
using WebEnterprise_1640.Models;

namespace WebEnterprise_1640.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class DetailsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DetailsController(ApplicationDbContext context)

        {
            _context = context;
        }
        [Route("[controller]/[action]")]
        public async Task<IActionResult> Index(int articleId)
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
            var article = _context.Articles.FirstOrDefault(a => a.Id == articleId);
            if (article != null)
            {
                article.Magazine = _context.Magazines.FirstOrDefault(m => m.Id == article.MagazineId);
                if (article.Magazine != null)
                {
                    article.Magazine.Faculty = _context.Faculties.FirstOrDefault(f => f.Id == article.Magazine.FacultyId);
                }
                article.Documents = _context.Documents.Where(d => d.ArticleId == article.Id).ToList();
            }
            ViewBag.User = user;
            ViewBag.UserRole = roleStr;
            return View(article);
        }
    }
}
