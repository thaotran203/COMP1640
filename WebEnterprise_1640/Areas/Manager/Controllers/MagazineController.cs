using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Index()
        {
            var magazines = _context.Magazines.Include(m => m.Faculty).Include(m => m.Semester);
            return View(await magazines.ToListAsync());
        }


        // GET: Manager/Magazine/Create
        public IActionResult Create()
        {
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

            ViewBag.FacultyList = _context.Faculties.ToList();
            ViewBag.SemesterList = _context.Semesters.ToList();
            return View(magazineModel);
        }

        private List<SelectListItem> GetFacultyList()
        {
            var faculties = _context.Faculties.ToList();
            return faculties.Select(f => new SelectListItem { Value = f.Id.ToString(), Text = f.Name }).ToList();
        }

        private List<SelectListItem> GetSemesterList()
        {
            var semesters = _context.Semesters.ToList();
            return semesters.Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name }).ToList();
        }

        // GET: Manager/Magazine/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Magazines == null)
            {
                return NotFound();
            }

            var magazineModel = await _context.Magazines.FindAsync(id);
            if (magazineModel == null)
            {
                return NotFound();
            }
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "Name", magazineModel.FacultyId);
            ViewData["SemesterId"] = new SelectList(_context.Semesters, "Id", "Name", magazineModel.SemesterId);
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
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "Name", magazineModel.FacultyId);
            ViewData["SemesterId"] = new SelectList(_context.Semesters, "Id", "Name", magazineModel.SemesterId);
            return View(magazineModel);
        }

        // GET: Manager/Magazine/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
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
