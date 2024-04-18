using Microsoft.AspNetCore.Mvc;
using WebEnterprise_1640.Data;
using WebEnterprise_1640.Models;

namespace WebEnterprise_1640.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FacultyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FacultyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Faculty
        public IActionResult Index()
        {
            List<FacultyModel> faculties = _context.Faculties.ToList();
            return View(faculties);
        }

        // GET: Admin/Faculty/Create
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(FacultyModel facultyModel)
        {
            if (ModelState.IsValid)
            {
                _context.Faculties.Add(facultyModel);
                _context.SaveChanges();
                TempData["success"] = "Created Faculty successfully";
                return RedirectToAction("Index");
            }
            return View();

        }
    }
}
