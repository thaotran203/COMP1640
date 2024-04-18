using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebEnterprise_1640.Data;
using WebEnterprise_1640.Models;
using System.Text.Json;

namespace WebEnterprise_1640.MagazineController
{
    [Area("Coordinator")]
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
            var magazines = await _dbContext.Magazines.Include(m => m.Semester).ToListAsync();
            return View("Index", magazines);
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
