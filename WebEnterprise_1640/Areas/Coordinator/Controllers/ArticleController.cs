using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebEnterprise_1640.Data;
using WebEnterprise_1640.Models;

namespace WebEnterprise_1640.ArticlesControllers
{
    [Area("Coordinator")]
    public class ArticleController : Controller
    {
        private readonly ILogger<ArticleController> _logger;
        private readonly ApplicationDbContext _context;

        public ArticleController(ILogger<ArticleController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<IActionResult> Index(int? magazineId, int page = 1, DateTime? searchDate = null, string searchQuery = "")
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
            var userRole = _context.UserRoles.FirstOrDefault(ur => ur.UserId == user.Id);
            if (userRole == null)
            {
                return Redirect("/Account/Login");
            }
            var role = _context.Roles.FirstOrDefault(r => r.Id == userRole.RoleId);
            if (role == null)
            {
                return Redirect("/Account/Login");
            }
            if (role.Name.ToLower() != "coordinator")
            {
                return Redirect("/Account/Login");
            }
            ViewBag.User = user;

            if (magazineId == null)
            {
                return NotFound();
            }

            const int pageSize = 4;

            var articlesQuery = _context.Articles
                .Where(a => a.MagazineId == magazineId && a.Status != "selected")
                .Include(a => a.User)
                .Include(a => a.Magazine)
                .OrderByDescending(a => a.SubmitDate);

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                articlesQuery = (IOrderedQueryable<Models.ArticleModel>)articlesQuery.Where(a => a.User.FullName.Contains(searchQuery) || a.Name.Contains(searchQuery));
            }

            if (searchDate != null)
            {
                articlesQuery = (IOrderedQueryable<Models.ArticleModel>)articlesQuery.Where(a => a.SubmitDate.Date == searchDate.Value.Date);
                ViewBag.SearchDate = searchDate.Value.ToString("MM/dd/yyyy");
            }
            else
            {
                ViewBag.SearchDate = null;
            }

            var totalArticles = await articlesQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalArticles / (double)pageSize);

            page = Math.Max(1, Math.Min(totalPages, page));

            var articles = await articlesQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.PageIndex = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchQuery = searchQuery;
            ViewBag.MagazineId = magazineId;

            return View("Index", articles);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int articleId, string status)
        {
            var article = await _context.Articles.FindAsync(articleId);
            if (article == null)
            {
                return NotFound();
            }

            article.Status = status;
            await _context.SaveChangesAsync();

            return Ok();
        }


    }
}
