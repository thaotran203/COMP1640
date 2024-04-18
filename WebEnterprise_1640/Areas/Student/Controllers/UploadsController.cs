using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebEnterprise_1640.Data;
using WebEnterprise_1640.Models;
using WebEnterprise_1640.Models.NewFolder;
using WebEnterprise_1640.Utility;

namespace WebEnterprise_1640.Areas.Student.Controllers
{
    [Area("Student")]
    public class UploadsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UploadsController(ApplicationDbContext context)

        {
            _context = context;
        }

        // 1 vào magazin để lấy thông tin bài báo 
        [Route("[controller]/[action]")]
        public IActionResult Index(search search)
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
            var manazines = new List<MagazineModel>();
            if (search.searchKey != null)
            {
                manazines = _context.Magazines.Where(x => x.Name.Contains(search.searchKey)).ToList();
            }
            else
            {
                manazines = _context.Magazines.ToList();
            }
            var semesters = _context.Semesters.ToList();
            var map = new List<SemesterModelView>();
            var manazines2 = new List<MagazineModel>();
            var manazines3 = new List<MagazineModel>();
            semesters.ForEach(i =>
            {
                var obj = new SemesterModelView();
                obj.Id = i.Id;
                obj.FinalClosureDate = i.FinalClosureDate.ToString("yyyy/MM/dd HH:mm");
                map.Add(obj);
            });
            manazines.ForEach(i =>
            {
                semesters.ForEach(e =>
                {
                    if (i.SemesterId == e.Id)
                    {
                        if (e.FinalClosureDate > DateTime.Now)
                        {
                            manazines2.Add(i);
                        }
                        else
                        {
                            manazines3.Add(i);
                        }
                    }
                });
            });
            ViewBag.Semeter = map;

            ViewBag.Manazines = manazines2;
            ViewBag.Manazine2 = manazines3;
            ViewBag.User = user;
            ViewBag.UserRole = roleStr;
            return View();
        }
    }
}
