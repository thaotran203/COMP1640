﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebEnterprise_1640.Data;
using WebEnterprise_1640.Models;
using WebEnterprise_1640.Utility;

namespace WebEnterprise_1640.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        public HomeController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager)

        {
            _context = context;
            _roleManager = roleManager;
        }

        [Route("[controller]/[action]")]
        public async Task<IActionResult> MainPage()
        {
            if (!_context.Roles.Any())
            {
                await _roleManager.CreateAsync(new IdentityRole { Id = "1", Name = "Guest" });
                await _roleManager.CreateAsync(new IdentityRole { Id = "2", Name = "Student" });
                await _roleManager.CreateAsync(new IdentityRole { Id = "3", Name = "Coordinator" });
                await _roleManager.CreateAsync(new IdentityRole { Id = "4", Name = "Manager" });
                await _roleManager.CreateAsync(new IdentityRole { Id = "5", Name = "Admin" });
            }

            return View();
        }
        [Route("[controller]/[action]")]
        public async Task<IActionResult> Index(int? facilityIdSort = null, string? search = null)
        {
            var userJson = HttpContext.Session.GetString("USER");
            UserModel? user = null;
            if (userJson != null && userJson.Length > 0)
            {
                user = JsonSerializer.Deserialize<UserModel>(userJson);
            }
            string roleStr = "";
            if (user != null)
            {
                var userRole = _context.UserRoles.FirstOrDefault(ur => ur.UserId == user.Id);
                if (userRole != null)
                {
                    var role = _context.Roles.FirstOrDefault(r => r.Id == userRole.RoleId);
                    if (role != null)
                    {
                        roleStr = role.Name;
                    }
                }
            }
            GUIUtils.CheckNotification(_context);
            var magazines = _context.Magazines.ToList();
            FacultyModel? curFacility = null;
            if (magazines != null && magazines.Count > 0)
            {
                if (user != null)
                {
                    magazines = magazines.Where(m => m.FacultyId == user.FacultyId).ToList();
                }
                if (facilityIdSort != null)
                {
                    magazines = magazines.Where(m => m.FacultyId == facilityIdSort).ToList();
                    curFacility = _context.Faculties.FirstOrDefault(f => f.Id == facilityIdSort);
                }
                if (search != null)
                {
                    magazines = magazines.Where(m => m.Name.ToLower().Contains(search.ToLower())).ToList();
                }
                foreach (var magazine in magazines)
                {
                    magazine.Articles = _context.Articles.Where(a => a.MagazineId == magazine.Id && a.Status.ToLower() == "selected".ToLower()).ToList();
                    if (magazine.Articles != null && magazine.Articles.Count > 0)
                    {
                        foreach (var article in magazine.Articles)
                        {
                            article.Documents = _context.Documents.Where(d => d.ArticleId == article.Id).ToList();
                        }
                    }
                }
            }
            var facilities = _context.Faculties.ToList();
            return View(new HomeViewModel()
            {
                Magazines = magazines,
                FacultyIdSort = facilityIdSort,
                Search = search,
                Faculties = facilities,
                CurFacility = curFacility,
                User = user,
                UserRole = roleStr
            });
        }
    }
}
