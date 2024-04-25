using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;
using WebEnterprise_1640.Data;
using WebEnterprise_1640.Models;

namespace WebEnterprise_1640.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class MagazineController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MagazineController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Manager/Magazine
        public async Task<IActionResult> Index(int? facultyId)
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

            IQueryable<MagazineModel> magazinesQuery = _context.Magazines.Include(m => m.Faculty).Include(m => m.Semester);

            if (facultyId.HasValue)
            {
                magazinesQuery = magazinesQuery.Where(m => m.FacultyId == facultyId.Value);
            }

            var magazines = await magazinesQuery.ToListAsync();
            ViewBag.Faculties = await _context.Faculties.ToListAsync();
            return View(magazines);
        }


        // GET: Manager/Magazine/Create
        public IActionResult Create()
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


            ViewBag.FacultyList = _context.Faculties.ToList();
            ViewBag.SemesterList = _context.Semesters.ToList();
            return View();
        }

        // POST: Manager/Magazine/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MagazineModel magazineModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(magazineModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Debug.WriteLine(error.ErrorMessage);
                }
            }

            ViewBag.FacultyList = _context.Faculties.ToList();
            ViewBag.SemesterList = _context.Semesters.ToList();
            return View(magazineModel);
        }


        // GET: Manager/Magazine/Edit/5
        public async Task<IActionResult> Edit(int? id)
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


            if (id == null || _context.Magazines == null)
            {
                return NotFound();
            }

            var magazineModel = await _context.Magazines.FindAsync(id);
            if (magazineModel == null)
            {
                return NotFound();
            }
            ViewBag.magazineId = magazineModel.Id;
            ViewBag.FacultyList = _context.Faculties.ToList();
            ViewBag.SemesterList = _context.Semesters.ToList();
            return View(magazineModel);
        }

        // POST: Manager/Magazine/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,ClosureDate,FacultyId,SemesterId")] MagazineModel magazineModel)
        {
            if (id != magazineModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(magazineModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MagazineModelExists(magazineModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.FacultyList = _context.Faculties.ToList();
            ViewBag.SemesterList = _context.Semesters.ToList();
            return View(magazineModel);
        }

        // GET: Manager/Magazine/Delete/5
        public async Task<IActionResult> Delete(int? id)
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


            if (id == null || _context.Magazines == null)
            {
                return NotFound();
            }

            var magazineModel = await _context.Magazines
                .Include(m => m.Faculty)
                .Include(m => m.Semester)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (magazineModel == null)
            {
                return NotFound();
            }

            return View(magazineModel);
        }

        // POST: Manager/Magazine/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Magazines == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Magazines'  is null.");
            }
            var magazineModel = await _context.Magazines.FindAsync(id);
            if (magazineModel != null)
            {
                _context.Magazines.Remove(magazineModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MagazineModelExists(int id)
        {
            return (_context.Magazines?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
