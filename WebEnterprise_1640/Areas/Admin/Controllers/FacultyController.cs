using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Index()
        {
              return _context.Faculties != null ? 
                          View(await _context.Faculties.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Faculties'  is null.");
        }

        // GET: Admin/Faculty/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Faculty/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FacultyModel facultyModel)
        {
			_context.Add(facultyModel);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

        // GET: Admin/Faculty/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Faculties == null)
            {
                return NotFound();
            }

            var facultyModel = await _context.Faculties.FindAsync(id);
            if (facultyModel == null)
            {
                return NotFound();
            }
            return View(facultyModel);
        }

        // POST: Admin/Faculty/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,FacultyModel facultyModel)
        {
            if (id != facultyModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(facultyModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacultyModelExists(facultyModel.Id))
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
            return View(facultyModel);
        }

        private bool FacultyModelExists(int id)
        {
          return (_context.Faculties?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
