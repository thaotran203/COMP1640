using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.Json;
using WebEnterprise_1640.Data;
using WebEnterprise_1640.Models;

namespace WebEnterprise_1640.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int selectedYear = -1, string selectedStatus = "selected")
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
            if (role.Name.ToLower() != "manager")
            {
                return Redirect("/Account/Login");
            }
            ViewBag.User = user;

            if (selectedYear == -1)
                selectedYear = DateTime.Now.Year;

            var itStudentCount = _context.Users.Count(u => u.Faculty.Name == "Information Technology");
            var graphicDesignStudentCount = _context.Users.Count(u => u.Faculty.Name == "Graphic Design");
            var businessStudentCount = _context.Users.Count(u => u.Faculty.Name == "Business");

            ViewBag.ITStudentCount = itStudentCount;
            ViewBag.GraphicDesignStudentCount = graphicDesignStudentCount;
            ViewBag.BusinessStudentCount = businessStudentCount;

            var totalSubmissionsForYear = _context.Articles
                .Count(a => a.SubmitDate.Year == selectedYear);


            var submissionsByMonth = _context.Articles
                .Where(a => a.SubmitDate.Year == selectedYear)
                .Join(_context.Users,
                    article => article.UserId,
                    user => user.Id,
                    (article, user) => new { article, user })
                .GroupBy(x => new
                {
                    Month = x.article.SubmitDate.Month,
                    Faculty = x.user.Faculty.Name
                })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    Faculty = g.Key.Faculty,
                    Count = g.Count()
                })
                .OrderBy(x => x.Month)
                .ToList();

            var allMonths = Enumerable.Range(1, 12);

            var labels = allMonths.Select(month => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)).ToList();

            var datasets = new Dictionary<string, List<int>>();
            foreach (var faculty in submissionsByMonth.Select(x => x.Faculty).Distinct())
            {
                var facultyCounts = new List<int>();
                foreach (var month in allMonths)
                {
                    var count = submissionsByMonth.FirstOrDefault(x => x.Faculty == faculty && x.Month == month)?.Count ?? 0;
                    facultyCounts.Add(count);
                }
                datasets.Add(faculty, facultyCounts);
            }

            var submissionPercentages = new List<double>();
            foreach (var faculty in datasets.Keys)
            {
                var submissionCountForYear = submissionsByMonth.Where(x => x.Faculty == faculty).Sum(x => x.Count);
                var percentage = submissionCountForYear * 100.0 / totalSubmissionsForYear;
                submissionPercentages.Add(percentage);
            }


            ViewBag.Labels = labels;
            ViewBag.Datasets = datasets;
            ViewBag.SubmissionPercentages = submissionPercentages;
            ViewBag.SelectedYear = selectedYear;

            return View();
        }

    }
}