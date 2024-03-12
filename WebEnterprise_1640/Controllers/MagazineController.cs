using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebEnterprise_1640.Models;
using System.Collections.Generic;
using System.Linq;
using WebEnterprise_1640.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace WebEnterprise_1640.Controllers
{
    public class MagazineController : Controller
    {
        private readonly ILogger<MagazineController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public MagazineController(ILogger<MagazineController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var magazines = await _dbContext.Magazines.ToListAsync();
            var semesters = await _dbContext.Semesters.ToListAsync();

            foreach (var magazine in magazines)
            {
                var semester = semesters.FirstOrDefault(s => s.Id == magazine.SemesterId);
                if (semester != null)
                {
                    magazine.FinalDeadline = semester.FinalClosureDate;
                }
            }

            return View(magazines);
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
