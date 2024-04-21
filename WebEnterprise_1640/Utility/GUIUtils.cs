using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Security.Policy;
using WebEnterprise_1640.Data;
using WebEnterprise_1640.Models;

namespace WebEnterprise_1640.Utility
{
    public class GUIUtils
    {
        public static async Task<DocumentModel> UploadImageAsync(IFormCollection formCollection)
        {
            string fileName = "";
            var document = new DocumentModel();
            if (formCollection.Files.Count > 0)
            {
                for (var i = 0; i < formCollection.Files.Count; i++)
                {
                    var list = formCollection.Files.ToArray();

                    var file = list[i];

                    var folderPath = Path.Combine("wwwroot\\media");
                    var pathToSave = Path.Combine(folderPath);
                    fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    if (file.ContentType.Contains("image"))
                    {
                        document.Image = fileName;
                    }
                    else
                    {
                        document.File = fileName;
                    }

                }

            }
            return document;
        }

        public static void CheckNotification(ApplicationDbContext context)
        {
            var articles = context.Articles.Where(a => a.Status.ToLower() == "Submited".ToLower()).Include(a => a.User).ToList();
            DateTime now = DateTime.Now;
            if(articles != null && articles.Count > 0)
            {
                string fromMail = "Khoantgcd201759@fpt.edu.vn";
                string fromPassword = "bahw jbpw zyio qbqd";
                string title = "";
                string body = "";
                foreach(var article in articles)
                {
                    var facility = context.Faculties.FirstOrDefault(f => f.Id == article.User.FacultyId);
                    if(facility == null)
                    {
                        continue;
                    }
                    var coordinator = context.Users.FirstOrDefault(u => u.Id == facility.CoordinatorId.ToString());
                    if (coordinator == null)
                    {
                        continue;
                    }
                    if (coordinator.FacultyId == article.User.FacultyId)
                    {
                        title = $"14 day notification email for {article.Name}";
                        body = $"<p>Student {article.User.FullName} submitted an article 14 days ago and still not be checked. Please comment and confirm the article for improvement!</p>";
                        DateTime next14Days = article.SubmitDate.AddDays(14);
                        if (now >= next14Days)
                        {
                            try
                            {
                                // send mail to coordinator
                                SendMail(fromMail, fromPassword, title, body, coordinator);
                                article.Status = "NotiSended";
                                context.Articles.Update(article);
                                context.SaveChanges();
                            } catch(Exception ex)
                            {
                            }
                        }
                    }
                }
            }
        }
        // mail gửi cho 2 người chủ nhiệm và người gửi
        public static void SendMail(string fromMail, string fromPassword, string title, string body, UserModel user)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = title;
            message.To.Add(new MailAddress(user.Email));//user.Email
            message.Body = body;
            message.IsBodyHtml = true;

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
