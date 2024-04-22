using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using WebEnterprise_1640.Data;

namespace WebEnterprise_1640.Areas.Coordinator.Controllers
{
	[Area("Coordinator")]
	public class DashboardController : Controller
	{
		private readonly ApplicationDbContext _context;

		public DashboardController(ApplicationDbContext context)
		{
			_context = context;
		}

		public IActionResult Index(int selectedYear = -1, string selectedStatus = "selected")
		{
			if (selectedYear == -1)
				selectedYear = DateTime.Now.Year;

			var submissionsByMonth = _context.Articles
				.Where(a => a.SubmitDate.Year == selectedYear)
				.GroupBy(a => a.SubmitDate.Month)
				.Select(g => new { Month = g.Key, Count = g.Count() })
				.OrderBy(g => g.Month)
				.ToList();

			var studentCount = _context.Users.Count();
			var commentCount = _context.Comments.Count();

			var selectedCount = _context.Articles.Count(a => a.Status == "selected");
			var deniedCount = _context.Articles.Count(a => a.Status == "denied");

			var totalArticleCount = selectedCount + deniedCount;
			var articlesWithSelectedStatusCount = _context.Articles
				.Count(a => a.Status == selectedStatus);

			var selectedPercentage = (double)selectedCount / totalArticleCount * 100.0;
			var deniedPercentage = (double)deniedCount / totalArticleCount * 100.0;

			var labels = Enumerable.Range(1, 12)
				.Select(i => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i))
				.ToList();
			var data = new List<int>(12); // Initialize with zeros
			foreach (var month in Enumerable.Range(1, 12))
			{
				var submissionCount = submissionsByMonth.FirstOrDefault(s => s.Month == month);
				data.Add(submissionCount != null ? submissionCount.Count : 0);
			}

			ViewBag.Labels = labels;
			ViewBag.Data = data;
			ViewBag.StudentCount = studentCount;
			ViewBag.CommentCount = commentCount;
			ViewBag.ArticlesWithSelectedStatusCount = articlesWithSelectedStatusCount;
			ViewBag.SelectedPercentage = selectedPercentage;
			ViewBag.DeniedPercentage = deniedPercentage;
			ViewBag.SelectedYear = selectedYear;

			return View();
		}
	}
}
