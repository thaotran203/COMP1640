using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebEnterprise_1640.Data;
using WebEnterprise_1640.Models;

namespace WebEnterprise_1640.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FacultyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public FacultyController(ApplicationDbContext context, UserManager<UserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Admin/Faculty
        public async Task<IActionResult> Index()
        {
            //Get user have roles want show
            var nonRoles = await _roleManager.Roles.Where(r => r.Name != "Admin" && r.Name != "Manager").Select(r => r.Name).ToListAsync();
            var users = new List<UserModel>();
            foreach (var user in _userManager.Users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                if (userRoles.Any(ur => nonRoles.Contains(ur)))
                {
                    users.Add(user);
                }
            }
            Dictionary<int, int> userCounts = new Dictionary<int, int>();
            //Count User
            List<FacultyModel> faculties = _context.Faculties.ToList();
            foreach (var faculty in faculties)
            {
                var accountCount = users.Where(u => u.FacultyId == faculty.Id).Count();
                userCounts.Add(faculty.Id, accountCount);
            }
            ViewBag.UserList = userCounts;
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
                TempData["success"] = "Faculty has been successfully created!";
                return RedirectToAction("Index");
            }
            return View();

        }

        // GET: Admin/Faculty/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            FacultyModel? facultyModel = _context.Faculties.Where(f => f.Id == id).FirstOrDefault();
            if (facultyModel == null)
            {
                return NotFound();
            }
            return View(facultyModel);
        }

        [HttpPost]
        public IActionResult Edit(FacultyModel facultyModel)
        {
            if (ModelState.IsValid)
            {
                _context.Faculties.Update(facultyModel);
                _context.SaveChanges();
                TempData["success"] = "Faculty has been successfully updated!";
                return RedirectToAction("Index");
            }
            return View();
        }

        // GET: Admin/Faculty/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null || id == null)
            {
                return NotFound();
            }

            FacultyModel? facultyModel = _context.Faculties.Where(f => f.Id == id).FirstOrDefault();
            if (facultyModel == null)
            {
                return NotFound();
            }

            return View(facultyModel);
        }
        [HttpPost]
        public IActionResult Delete(FacultyModel facultyModel)
        {
            _context.Faculties.Remove(facultyModel);
            _context.SaveChanges();
            TempData["success"] = "Faculty has been successfully deleted!";
            return RedirectToAction("Index");

        }
    }
}