using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebEnterprise_1640.Models;

namespace WebEnterprise_1640.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(string url)
        {
            if(url != null && url.Length > 0 )
            {
                return Redirect(url);
            } else
            {
                return RedirectToAction("MainPage", "Home");
            }
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