using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebEnterprise_1640.Data;
using WebEnterprise_1640.Models;

namespace WebEnterprise_1640.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class SemesterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SemesterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Manager/Semester
        public async Task<IActionResult> Index()
        {
            return View(await _context.Semesters.ToListAsync());
        }


        // GET: Manager/Semester/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Manager/Semester/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SemesterModel semesterModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(semesterModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(semesterModel);
        }

        // GET: Manager/Semester/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Semesters == null)
            {
                return NotFound();
            }

            var semesterModel = await _context.Semesters.FindAsync(id);
            if (semesterModel == null)
            {
                return NotFound();
            }
            return View(semesterModel);
        }

        // POST: Manager/Semester/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SemesterModel semesterModel)
        {
            if (id != semesterModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(semesterModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SemesterModelExists(semesterModel.Id))
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
            return View(semesterModel);
        }

        // GET: Manager/Semester/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Semesters == null)
            {
                return NotFound();
            }

            var semesterModel = await _context.Semesters
                .FirstOrDefaultAsync(m => m.Id == id);
            if (semesterModel == null)
            {
                return NotFound();
            }

            return View(semesterModel);
        }

        // POST: Manager/Semester/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Semesters == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Semesters'  is null.");
            }
            var semesterModel = await _context.Semesters.FindAsync(id);
            if (semesterModel != null)
            {
                _context.Semesters.Remove(semesterModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SemesterModelExists(int id)
        {
            return (_context.Semesters?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
