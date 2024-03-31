using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebEnterprise_1640.Data;

namespace WebEnterprise_1640.ArticlesControllers
{
    [Area("Coordinator")]
    public class SelectedArticleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SelectedArticleController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? magazineId, int page = 1)
        {
            if (magazineId == null)
            {
                return NotFound();
            }

            const int pageSize = 4;

            var articlesQuery = _context.Articles
                .Where(a => a.MagazineId == magazineId && a.Status == "selected")
                .Include(a => a.User)
                .Include(a => a.Magazine)
                .OrderByDescending(a => a.SubmitDate);

            var totalArticles = await articlesQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalArticles / (double)pageSize);

            page = Math.Max(1, Math.Min(totalPages, page));

            var articles = await articlesQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.PageIndex = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.MagazineId = magazineId;

            return View(articles);
        }

    }
}
