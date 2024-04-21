
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebEnterprise_1640.Data;
using WebEnterprise_1640.Models;
using WebEnterprise_1640.Models.NewFolder;

namespace WebEnterprise_1640.Areas.Student.Controllers
{
    [Area("Student")]
    public class SubmitArticleController : Controller
    {
        private readonly ApplicationDbContext _context;
        public SubmitArticleController(ApplicationDbContext context)

        {
            _context = context;
        }

        [Route("[controller]/[action]")]
        public IActionResult Index(ArticleViewModel input)
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
            input.Comments = _context.Comments.Where(c => c.ArticleId == input.Id).Include(c => c.User).ToList();
            var file = _context.Documents.Where(x => x.ArticleId == input.Id).ToList();
            ViewBag.Articles = input;
            // Ngày hiện tại
            DateTime currentDate = DateTime.Now;

            // Chuỗi ngày đầu vào
            string inputDateStr = ViewBag.Articles.TimeEnd;
            ViewBag.check = 0;

            // Chuyển đổi chuỗi ngày đầu vào thành đối tượng DateTime
            DateTime inputDate;
            if (DateTime.TryParseExact(inputDateStr, "yyyy/MM/dd HH:mm", null, System.Globalization.DateTimeStyles.None, out inputDate))
            {
                // So sánh
                int result = DateTime.Compare(inputDate, currentDate);

                if (result < 0)
                {
                    ViewBag.check = 1;
                }
                else if (result == 0)
                {
                    ViewBag.check = 2;
                }
                else
                {
                    ViewBag.check = 3;
                }
            }

            ViewBag.File = file;
            ViewBag.User = user;
            return View(input);
        }

        [Route("[controller]/[action]")]
        public async Task<IActionResult> Delete(int id)
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
            var data = _context.Articles.FirstOrDefault(x => x.Id == id);
            var file = _context.Documents.Where(i => i.ArticleId == data.Id).ToList();
            if (file != null && file.Count > 0)
            {
                file.ForEach(async x =>
                {
                    _context.Documents.Remove(x);
                });
            }
            await _context.SaveChangesAsync();
            _context.Articles.Remove(data);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Uploads");
        }

        [Route("[controller]/[action]")]
        public async Task<IActionResult> RemoveArticle(int id)
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
            var data = _context.Articles.FirstOrDefault(x => x.Id == id);
            ViewBag.ArticleId = id;
            return View();
        }

        [Route("[controller]/[action]")]
        public async Task<IActionResult> DeleteFile(int id, string type, int articleId)
        {
            //phan quyen
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
            var file = _context.Documents.FirstOrDefault(i => i.Id == id);
            if (type == "file")
            {
                file.File = "";
            }
            else
            {
                file.Image = "";
            }
            var tempDocs = new List<DocumentModel>();
            var tempDocsJson = HttpContext.Session.GetString("TEMP_DOCS");
            if (tempDocsJson == null)
            {
                tempDocs.Add(file);
            }
            else
            {
                tempDocs = JsonSerializer.Deserialize<List<DocumentModel>>(tempDocsJson);
                if (tempDocs != null && tempDocs.Count > 0)
                {
                    tempDocs.Add(file);
                }
                else
                {
                    tempDocs = new List<DocumentModel>();
                    tempDocs.Add(file);
                }
            }
            HttpContext.Session.SetString("TEMP_DOCS", JsonSerializer.Serialize(tempDocs));
            return RedirectToAction("Index", "Edit", new { id = articleId });
        }

        [Route("[controller]/[action]")]
        public async Task<IActionResult> DeleteView(int id, string type, int articleId)
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
            var file = _context.Documents.FirstOrDefault(i => i.Id == id);
            ViewBag.File = file;
            ViewBag.Type = type;
            ViewBag.ArticleId = articleId;
            return View();
        }
        [Route("[controller]/[action]")]
        public async Task<IActionResult> AddComment(int articleId, string comment)
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
            var article = _context.Articles.FirstOrDefault(a => a.Id == articleId);
            if (article == null)
            {
                TempData["ErrorMessage"] = "Not Found Article!";
                return RedirectToAction("Index", "Uploads");
            }
            if (comment == null || comment.Length == 0)
            {
                return RedirectToAction("Index", "GetById", new { id = article.MagazineId });
            }
            var newComment = new CommentModel()
            {
                ArticleId = articleId,
                UserId = user.Id,
                Content = comment,
            };
            var nComment = _context.Comments.Add(newComment);
            await _context.SaveChangesAsync();
            if (nComment == null || nComment.Entity == null)
            {
                TempData["ErrorMessage"] = "Database Connection Error!";
                return RedirectToAction("Index", "GetById", new { id = article.MagazineId });
            }
            return RedirectToAction("Index", "GetById", new { id = article.MagazineId });
        }
    }
}
