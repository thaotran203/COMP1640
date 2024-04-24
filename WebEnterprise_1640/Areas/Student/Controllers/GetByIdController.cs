using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Text.Json;
using WebEnterprise_1640.Data;
using WebEnterprise_1640.Models;
using WebEnterprise_1640.Models.NewFolder;
using WebEnterprise_1640.Utility;

namespace WebEnterprise_1640.Areas.Student.Controllers
{
    [Area("Student")]
    public class GetByIdController : Controller
    {
        private readonly ApplicationDbContext _context;
        public GetByIdController(ApplicationDbContext context)

        {
            _context = context;
        }

        [Route("[controller]/[action]")]
        public async Task<IActionResult> Index(int id)
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
            if (role.Name.ToLower() != "student")
            {
                return Redirect("/Account/Login");
            }
            //Lấy thông tin từ cơ sở dữ liệu của magazin và điều chỉnh lại thời gian thành kiểu chuỗi để in ra dữ liệu
            HttpContext.Session.Remove("TEMP_DOCS");
            var magazines = _context.Magazines.FirstOrDefault(x => x.Id == id);
            var timeEnd = _context.Semesters.FirstOrDefault(x => x.Id == magazines.SemesterId);
            ViewBag.User = user;
            ViewBag.MagazineId = id;
            ViewBag.Magazines = magazines;
            ViewBag.TimeStart = magazines.ClosureDate.ToString("yyyy/MM/dd HH:mm");
            ViewBag.TimeEnd = timeEnd.FinalClosureDate.ToString("yyyy/MM/dd HH:mm");
            var daynow = DateTime.UtcNow.Date;
            DateTime dayEnd = timeEnd.FinalClosureDate;
            ViewBag.Deadline = DateTime.Compare(daynow, dayEnd);
            //kiểm tra xem có bài báo nào đã được nộp
            //nếu đã nộp thì sẽ chạy sang trang đã submit
            var check = _context.Articles.FirstOrDefault(x => x.MagazineId == magazines.Id);

            if (check != null)
            {
                var file = _context.Documents.Where(x => x.ArticleId == check.Id).ToList();
                var map = new ArticleViewModel();
                map.Id = check.Id;
                map.UserId = check.UserId;
                map.Name = check.Name;
                map.Description = check.Description;
                map.Status = check.Status;
                map.MagazineId = magazines.Id;
                map.TimeEnd = timeEnd.FinalClosureDate.ToString("yyyy/MM/dd HH:mm");
                map.TimeStart = magazines.ClosureDate.ToString("yyyy/MM/dd HH:mm");
                map.TimeSubmit = check.SubmitDate.ToString("yyyy/MM/dd HH:mm");
                map.File = file;
                return RedirectToAction("Index", "SubmitArticle", map);
            }
            //nếu chưa sẽ chạy sang view tạo mới bài viết CreateArtice
            else
            {
                return View();
            }

        }

        public string CreateCookie(string cookieValue)
        {
            HttpContext.Response.Cookies.Append("user_id", "1");

            var userId = "";

            userId = HttpContext.Request.Cookies["user_id"];

            if (userId != null)
            {
                return userId;
            }
            else
            {
                return null;
            }
        }
        [Route("[controller]/[action]")]
        public async Task<IActionResult> Create(ArticleModel input)
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
            if (role.Name.ToLower() != "student")
            {
                return Redirect("/Account/Login");
            }
            if (string.IsNullOrEmpty(input.Name))
            {
                TempData["ErrorMessage"] = "Title must not empty!";
                return RedirectToAction("Index","GetbyId", new { id = input.MagazineId });
            }
            if (string.IsNullOrEmpty(input.Description))
            {
                TempData["ErrorMessage"] = "Description must not empty!";
                return RedirectToAction("Index", "GetbyId", new { id = input.MagazineId });
            }
            var userId = CreateCookie("1");
            var formfile = await Request.ReadFormAsync();
            if (formfile == null || formfile.Files.Count == 0)
            {
                TempData["ErrorMessage"] = "Choose one file pdf and one image!";
                return RedirectToAction("Index", "GetbyId", new { id = input.MagazineId });
            }
            bool isHaveDoc = false;
            bool isHaveImage = false;
            for (var i = 0; i < formfile.Files.Count; i++)
            {
                var list = formfile.Files.ToArray();
                var file = list[i];
                if (file.FileName.ToLower().Contains("png") || file.FileName.ToLower().Contains("jpg"))
                {
                    isHaveImage = true;
                }
                else if (file.FileName.ToLower().Contains("pdf") || file.FileName.ToLower().Contains("docx") || file.FileName.ToLower().Contains("doc"))
                {
                    isHaveDoc = true;
                }
            }
            if (!isHaveDoc || !isHaveImage)
            {
                TempData["ErrorMessage"] = "Choose one file pdf and one image!";
                return RedirectToAction("Index", "GetbyId", new { id = input.MagazineId });
            }
            input.UserId = user.Id;
            input.Status = "submited";
            input.SubmitDate = DateTime.Now;
            var articleAdd = _context.Articles.Add(input);
            await _context.SaveChangesAsync();
            var lastArticle = _context.Articles.ToList().Last();
            var document = new DocumentModel();
            document = await GUIUtils.UploadImageAsync(formfile);
            document.ArticleId = lastArticle.Id;
            if (document.Image == null)
            {
                document.Image = "";
            }
            if (document.File == null)
            {
                document.File = "";
            }
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();
            //send mail to sender
            string fromMail = "Khoantgcd201759@fpt.edu.vn";
            string fromPassword = "bahw jbpw zyio qbqd";
            var facility = _context.Faculties.FirstOrDefault(f => f.Id == user.FacultyId);
            string url = Url.ActionLink("Index", "GetbyId", new { id = input.MagazineId });
            string title = $"Article submission notification for {input.Name}";
            string body = $"<p>Student {user.UserName} have submitted an article. Please check and confirm this article!</p>";
            if (facility != null)
            {
                var coordinator = _context.Users.FirstOrDefault(u => u.Id == facility.CoordinatorId.ToString());
                if (coordinator != null)
                {
                    if (coordinator.FacultyId == user.FacultyId)
                    {
                        // send mail to coordinator
                        GUIUtils.SendMail(fromMail, fromPassword, title, body, coordinator);
                    }
                }
            }
            // send mail to sender
            GUIUtils.SendMail(fromMail, fromPassword, title, body, user);

            return RedirectToAction("Index", "Uploads");
        }
    }
}
