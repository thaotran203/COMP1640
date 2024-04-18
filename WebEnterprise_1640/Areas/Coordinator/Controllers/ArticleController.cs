using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebEnterprise_1640.Data;

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

        public async Task<IActionResult> Index(int? magazineId, int page = 1, string searchQuery = "")
        {
            if (magazineId == null)
            {
                return NotFound();
            }

            const int pageSize = 4;

            var articlesQuery = _context.Articles
                .Where(a => a.MagazineId == magazineId)
                .Include(a => a.User)
                .Include(a => a.Magazine)
                .OrderByDescending(a => a.SubmitDate);

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                articlesQuery = (IOrderedQueryable<Models.ArticleModel>)articlesQuery.Where(a => a.User.FullName.Contains(searchQuery) || a.Name.Contains(searchQuery));
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

    }
}
