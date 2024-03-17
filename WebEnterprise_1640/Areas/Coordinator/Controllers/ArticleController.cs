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

		// GET: Articles
		public async Task<IActionResult> Index(int? magazineId, int page = 1)
		{
			if (magazineId == null)
			{
				return NotFound();
			}

			const int pageSize = 4;

			// Initial query with ordering
			var articlesQuery = _context.Articles
				.Where(a => a.MagazineId == magazineId)
				.Include(a => a.User)
				.Include(a => a.Magazine)
				.OrderByDescending(a => a.SubmitDate);

			// Continue with pagination and final query execution
			var totalArticles = await articlesQuery.CountAsync();
			var totalPages = (int)Math.Ceiling(totalArticles / (double)pageSize);

			// Ensure page number is within bounds
			page = Math.Max(1, Math.Min(totalPages, page));

			var articles = await articlesQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

			ViewBag.PageIndex = page;
			ViewBag.TotalPages = totalPages;
			ViewBag.MagazineId = magazineId; // Make sure to include MagazineId

			return View("Index", articles);
		}

	}
}
