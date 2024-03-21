using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebEnterprise_1640.Data;
using WebEnterprise_1640.Models;

namespace WebEnterprise_1640.Controllers
{
    [Area("Coordinator")]
    public class ArticleController : Controller
    {
        private readonly ILogger<ArticleController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public ArticleController(ILogger<ArticleController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index(int? id)
        {
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

            return View(new List<ArticleModel> { article });
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
