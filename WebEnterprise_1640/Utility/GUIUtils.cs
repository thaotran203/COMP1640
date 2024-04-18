using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Security.Policy;
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
        // mail gửi cho 2 người chủ nhiệm và người gửi
        public static void SendMail(string fromMail, string fromPassword, UserModel user, ArticleModel input, string submissionLink)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = $"You have submitted your article submission for {input.Name}";
            message.To.Add(new MailAddress(user.Email));//user.Email
            message.Body = $"<p><a href=\"https://localhost:44345/\">WebEnterprise_1640</a></p>"
                         + $"<hr/>"
                         + $"<p>You have summitted an article submission for {input.Name}.</p>"
                         + $"<p>You can see the status of your <a href=\"{submissionLink}\">article submission</a></p>" +
                           $"<hr/>";
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
