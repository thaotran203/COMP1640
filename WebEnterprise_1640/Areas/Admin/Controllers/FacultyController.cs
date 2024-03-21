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
        public IActionResult Index(int? id)
        {
            List<FacultyModel> faculties = _context.Faculties.ToList();
            ViewBag.UserList = _context.Users.Where(u => u.FacultyId == id).Count();
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

        // POST: Admin/Faculty/Delete/5
        [HttpPost]
        public IActionResult Delete(FacultyModel facultyModel)
        {
            _context.Faculties.Remove(facultyModel);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}