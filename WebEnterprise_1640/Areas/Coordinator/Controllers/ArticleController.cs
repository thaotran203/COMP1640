using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.Text.Json;
using WebEnterprise_1640.Data;
using WebEnterprise_1640.Models;

namespace WebEnterprise_1640.ArticlesControllers
{
    [Area("Coordinator")]
    public class ArticleController : Controller
    {
        private readonly ILogger<ArticleController> _logger;
        private readonly ApplicationDbContext _context;

        public ArticleController(ILogger<ArticleController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(int? magazineId, int page = 1, DateTime? searchDate = null, string searchQuery = "")
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
            if (role.Name.ToLower() != "coordinator")
            {
                return Redirect("/Account/Login");
            }
            ViewBag.User = user;

            if (magazineId == null)
            {
                return NotFound();
            }

            const int pageSize = 4;

            var articlesQuery = _context.Articles
                .Where(a => a.MagazineId == magazineId)
                .Include(a => a.User)
                .Include(a => a.Magazine)
                .OrderByDescending(a => a.SubmitDate);

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                articlesQuery = (IOrderedQueryable<Models.ArticleModel>)articlesQuery.Where(a => a.User.FullName.Contains(searchQuery) || a.Name.Contains(searchQuery));
            }

            if (searchDate != null)
            {
                articlesQuery = (IOrderedQueryable<Models.ArticleModel>)articlesQuery.Where(a => a.SubmitDate.Date == searchDate.Value.Date);
                ViewBag.SearchDate = searchDate.Value.ToString("MM/dd/yyyy");
            }
            else
            {
                ViewBag.SearchDate = null;
            }

            var totalArticles = await articlesQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalArticles / (double)pageSize);

            page = Math.Max(1, Math.Min(totalPages, page));

            var articles = await articlesQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.PageIndex = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchQuery = searchQuery;
            ViewBag.MagazineId = magazineId;

            return View("Index", articles);
        }

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
            if (role.Name.ToLower() != "coordinator")
            {
                return Redirect("/Account/Login");
            }
            var article = _context.Articles
                .Include(a => a.User)
                .FirstOrDefault(a => a.Id == articleId);
            if (article != null)
            {
                var articleOwnerEmail = article.User.Email;

                // Send email notification if the article owner's email is available
                if (!string.IsNullOrEmpty(articleOwnerEmail))
                {
                    SendEmailNotification(articleOwnerEmail, "New Comment on Your Article", "Coordinator has commented on your article.", user.FullName, comment);
                }
            }
            if (article == null)
            {
                TempData["ErrorMessage"] = "Not Found Article!";
                return RedirectToAction("Index", "Articles", new { id = articleId });
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
            }
            return RedirectToAction("Index", "Articles", new { id = articleId });
        }
        private void SendEmailNotification(string toEmail, string subject, string body, string commenterName, string commentContent)
        {
            string fromMail = "beemagazine3@gmail.com";
            string fromPassword = "aulftywznetqjinz";

            // Combine the original body with the commenter's name and comment content
            body += $"<br/><br/><b>New Comment by {commenterName}:</b><br/>{commentContent}";

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.To.Add(new MailAddress(toEmail));
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true; // Ensure HTML formatting for the body

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };

            smtpClient.Send(message);
        }
    }
}
