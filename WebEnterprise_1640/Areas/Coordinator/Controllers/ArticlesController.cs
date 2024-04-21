using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;
using WebEnterprise_1640.Data;
using WebEnterprise_1640.Models;

namespace WebEnterprise_1640.Controllers
{
    [Area("Coordinator")]
    public class ArticlesController : Controller
    {
        private readonly ILogger<ArticlesController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public ArticlesController(ILogger<ArticlesController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index(int? id)
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


            if (id == null)
            {
                return NotFound();
            }

            var article = _dbContext.Articles
    .Include(a => a.User)
    .Include(a => a.Magazine)
    .FirstOrDefault(a => a.Id == id);

            if (article == null)
            {
                return NotFound();
            }

            article.Documents = _dbContext.Documents.Where(d => d.ArticleId == article.Id).ToList();
            article.Comments = _dbContext.Comments.Where(c => c.ArticleId == article.Id).Include(c => c.User).ToList();

            return View("Article", new List<ArticleModel> { article });
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